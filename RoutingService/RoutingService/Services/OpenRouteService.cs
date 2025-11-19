using Newtonsoft.Json.Linq; 
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace RoutingService.Services
{
    public class OpenRouteService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        private readonly string _apiKey = "eyJvcmciOiI1YjNjZTM1OTc4NTExMTAwMDFjZjYyNDgiLCJpZCI6IjU3MzhmYjM1MTg4ODQxMDliMzNlOThlZmM3ZDY2YzQzIiwiaCI6Im11cm11cjY0In0=";

        public async Task<(double longitude, double latitude, string city)> GetCoordinatesAsync(string address)
        {
            string safeAddress = Uri.EscapeDataString(address); //encodage adresse pour url

            string requestUrl = $"https://api.openrouteservice.org/geocode/search?api_key={_apiKey}&text={safeAddress}&size=1";

            try
            {
             
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);

                
                var feature = data["features"][0]; 
                var coordinates = feature["geometry"]["coordinates"];

                
                double longitude = (double)coordinates[0];
                double latitude = (double)coordinates[1];

                
                string city = (string)feature["properties"]["locality"] ?? (string)feature["properties"]["county"];

                
                return (longitude, latitude, city?.ToLower()); // On met en minuscules pour JCDecaux
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Geocoding : {ex.Message}");
                return (0, 0, null);
            }
        }


        /// <summary>
        /// Calcule la durée d'un itinéraire entre deux points GPS.
        /// </summary>
        /// <param name="start">Coordonnées (Lon, Lat) de départ</param>
        /// <param name="end">Coordonnées (Lon, Lat) d'arrivée</param>
        /// <param name="profile">Profil de transport (ex: "foot-walking", "cycling-regular")</param>
        /// <returns>La durée du trajet en SECONDES.</returns>
        public async Task<double> GetRouteDurationAsync(
            (double lon, double lat) start,
            (double lon, double lat) end,
            string profile = "foot-walking")
        {
            
            
            string startPoint = $"{start.lon.ToString(System.Globalization.CultureInfo.InvariantCulture)},{start.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            string endPoint = $"{end.lon.ToString(System.Globalization.CultureInfo.InvariantCulture)},{end.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            string requestUrl = $"https://api.openrouteservice.org/v2/directions/{profile}?api_key={_apiKey}&start={startPoint}&end={endPoint}";

            try
            {
           
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                
                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);

                
                double durationInSeconds = (double)data["features"][0]["properties"]["summary"]["duration"];

                return durationInSeconds;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Routing ({profile}) : {ex.Message}");
                return -1;
            }
        }

        

        /// <summary>
        /// Récupère à la fois la DURÉE et la GÉOMÉTRIE (les points du tracé).
        /// </summary>
        public async Task<(double duration, List<double[]> geometry)> GetRouteDataAsync(
            (double lon, double lat) start,
            (double lon, double lat) end,
            string profile = "foot-walking")
        {
            string startPoint = $"{start.lon.ToString(System.Globalization.CultureInfo.InvariantCulture)},{start.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}";
            string endPoint = $"{end.lon.ToString(System.Globalization.CultureInfo.InvariantCulture)},{end.lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}";

            
            string requestUrl = $"https://api.openrouteservice.org/v2/directions/{profile}?api_key={_apiKey}&start={startPoint}&end={endPoint}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                string jsonResponse = await response.Content.ReadAsStringAsync();
                JObject data = JObject.Parse(jsonResponse);

                
                double duration = (double)data["features"][0]["properties"]["summary"]["duration"];

                
                var coordsJson = data["features"][0]["geometry"]["coordinates"];
                List<double[]> geometry = coordsJson.ToObject<List<double[]>>();

                

                return (duration, geometry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur Routing complet ({profile}) : {ex.Message}");
                return (0, new List<double[]>());
            }
        }
    }
}