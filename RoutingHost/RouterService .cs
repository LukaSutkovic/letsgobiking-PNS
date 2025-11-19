using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;         
using ProxyCore.Dto;

namespace RoutingHost
{
    public sealed class RouterService : IRouter, IDisposable
    {
        private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

        private readonly HttpClient _proxy = new HttpClient { BaseAddress = new Uri("http://localhost:9001/api/") };
        private readonly HttpClient _osrm = new HttpClient { BaseAddress = new Uri("https://router.project-osrm.org/") };

        public void Dispose() { _proxy?.Dispose(); _osrm?.Dispose(); }

        public async Task<RouteResponse> GetRoute(string origin, string dest)
        {
            // Parse origin/dest "lat,lon"
            var (olat, olon) = ParseLatLon(origin);
            var (dlat, dlon) = ParseLatLon(dest);
            var o = new GeoCoordinate(olat, olon);
            var d = new GeoCoordinate(dlat, dlon);

            // 1) Contrats via proxy
            var contracts = await GetAsync<List<Contract>>(_proxy, "contracts");

            // 2) Stations candidates: on parcourt les contrats, on retient celles à < 10 km de O ou D
            var nearO = new List<Station>();
            var nearD = new List<Station>();
            foreach (var c in contracts)
            {
                var stations = await GetAsync<List<Station>>(_proxy, $"stations?contract={Uri.EscapeDataString(c.Name)}");
                foreach (var s in stations.Where(s => s?.Position != null))
                {
                    var g = new GeoCoordinate(s.Position.Latitude, s.Position.Longitude);
                    var distO = o.GetDistanceTo(g);
                    var distD = d.GetDistanceTo(g);
                    if (distO <= 10_000) nearO.Add(s);
                    if (distD <= 10_000) nearD.Add(s);
                }
            }
            if (nearO.Count == 0 || nearD.Count == 0)
                return new RouteResponse { usingBike = false, rationale = "Aucune station proche (<10 km) de l’un des points." };

            // 3) Choix stations les plus proches (MVP: ne gère pas encore la dispo temps réel)
            var pickup = MinBy(nearO, s => o.GetDistanceTo(new GeoCoordinate(s.Position.Latitude, s.Position.Longitude)));
            var drop = MinBy(nearD, s => d.GetDistanceTo(new GeoCoordinate(s.Position.Latitude, s.Position.Longitude)));

            var p = new GeoCoordinate(pickup.Position.Latitude, pickup.Position.Longitude);
            var q = new GeoCoordinate(drop.Position.Latitude, drop.Position.Longitude);

            // 4) Itinéraires via OSRM
            var walkOD = await OsrmRoute("foot", o, d);
            var walkOP = await OsrmRoute("foot", o, p);
            var bikePQ = await OsrmRoute("bike", p, q);
            var walkQD = await OsrmRoute("foot", q, d);

            var bikeTotal = walkOP.durationSeconds + bikePQ.durationSeconds + walkQD.durationSeconds;

            // 5) Décision: prendre le vélo si on gagne ≥ 15%
            var useBike = bikeTotal <= 0.85 * walkOD.durationSeconds;

            return new RouteResponse
            {
                usingBike = useBike,
                totalDistanceMeters = useBike ? (walkOP.distanceMeters + bikePQ.distanceMeters + walkQD.distanceMeters)
                                              : walkOD.distanceMeters,
                totalDurationSeconds = useBike ? bikeTotal : walkOD.durationSeconds,
                walkingToPickup = useBike ? walkOP : null,
                bikeLeg = useBike ? bikePQ : null,
                walkingToDest = useBike ? walkQD : walkOD,
                pickupStation = useBike ? new RouteResponse.StationRef
                {
                    number = pickup.Number,
                    name = pickup.Name,
                    lat = pickup.Position.Latitude,
                    lon = pickup.Position.Longitude
                } : null,
                dropStation = useBike ? new RouteResponse.StationRef
                {
                    number = drop.Number,
                    name = drop.Name,
                    lat = drop.Position.Latitude,
                    lon = drop.Position.Longitude
                } : null,
                rationale = useBike ? "Vélo plus rapide de ≥15%." : "Marche directe plus compétitive."
            };
        }

        // --- Helpers ----------------------------------------------------------
        private static (double lat, double lon) ParseLatLon(string s)
        {
            var parts = s.Split(',');
            return (double.Parse(parts[0], Inv), double.Parse(parts[1], Inv));
        }

        private static T MinBy<T>(IEnumerable<T> xs, Func<T, double> key) =>
            xs.Aggregate((best, cur) => key(cur) < key(best) ? cur : best);

        private static async Task<T> GetAsync<T>(HttpClient http, string rel)
        {
            using (var resp = await http.GetAsync(rel))
            {
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            }
        }

        private static async Task<RouteResponse.Leg> OsrmRoute(string mode, System.Device.Location.GeoCoordinate a, System.Device.Location.GeoCoordinate b)
        {
            var profile = mode == "bike" ? "cycling" : "foot";
            var url = $"route/v1/{profile}/{a.Longitude.ToString(Inv)},{a.Latitude.ToString(Inv)};{b.Longitude.ToString(Inv)},{b.Latitude.ToString(Inv)}?overview=false&steps=true";

            using (var http = new HttpClient { BaseAddress = new Uri("https://router.project-osrm.org/") })
            using (var resp = await http.GetAsync(url))
            {
                resp.EnsureSuccessStatusCode();
                var json = await resp.Content.ReadAsStringAsync();
                dynamic o = Newtonsoft.Json.JsonConvert.DeserializeObject(json);
                if (o.code != "Ok") throw new InvalidOperationException("OSRM error");

                double dist = o.routes[0].distance;
                double dura = o.routes[0].duration;

                var steps = new System.Collections.Generic.List<string>();
                foreach (var leg in o.routes[0].legs)
                    foreach (var s in leg.steps)
                        steps.Add((string)s.maneuver.instruction);

                return new RouteResponse.Leg { distanceMeters = dist, durationSeconds = dura, instructions = steps };
            }
        }
    }
}
