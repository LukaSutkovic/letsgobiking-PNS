using RoutingService.Services; // N'oublie pas d'importer tes services
using RoutingService.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RoutingService.Services
{
    public class ItineraryService
    {
        // Notre nouveau service a besoin des "anciens" pour travailler
        private readonly OpenRouteService _orsService = new OpenRouteService();
        private readonly JCDecauxService _jcdService = new JCDecauxService();

        // (DANS LA CLASSE ItineraryService)

        /// <summary>
        /// C'est la méthode "chef d'orchestre".
        /// Elle gère maintenant la logique [Légale].
        /// </summary>
        // (DANS ItineraryService.cs)

        public async Task<RouteLeg> CalculateItineraryAsync(string depart, string arrivee)
        {
            // 1. Obtenir les données
            (double lon, double lat, string city) departData = await _orsService.GetCoordinatesAsync(depart);
            if (string.IsNullOrEmpty(departData.city)) return new RouteLeg(0, $"Adresse de départ non trouvée : {depart}");

            (double lon, double lat, string city) arriveeData = await _orsService.GetCoordinatesAsync(arrivee);
            if (string.IsNullOrEmpty(arriveeData.city)) return new RouteLeg(0, $"Adresse d'arrivée non trouvée : {arrivee}");

            string contractDepart = departData.city;
            string contractArrivee = arriveeData.city;

            // --- CAS 1 : MÊME CONTRAT ---
            if (contractDepart == contractArrivee)
            {
                List<Station> stations = await _jcdService.GetStationsAsync(contractDepart);
                return await CalculateBestTimeInContractAsync(
                    (departData.lon, departData.lat),
                    (arriveeData.lon, arriveeData.lat),
                    stations);
            }
            // --- CAS 2 : CONTRATS DIFFÉRENTS (LÉGAL) ---
            else
            {
                // 1. Référence marche (AVEC GÉOMÉTRIE)
                var walkData = await _orsService.GetRouteDataAsync(
                    (departData.lon, departData.lat),
                    (arriveeData.lon, arriveeData.lat));

                List<Station> stationsDepart = await _jcdService.GetStationsAsync(contractDepart);
                List<Station> stationsArrivee = await _jcdService.GetStationsAsync(contractArrivee);

                if (stationsDepart.Count == 0 || stationsArrivee.Count == 0)
                {
                    string msg = $"[Solution : Marche 🚶] (Pas de stations)\nDe: {contractDepart} | Vers: {contractArrivee}\nTemps total : {walkData.duration / 60:F1} minutes.";
                    return new RouteLeg(walkData.duration, msg, walkData.geometry);
                }

                Station exitStation = stationsDepart
                    .OrderBy(s => GeoUtils.GetDistance(arriveeData.lat, arriveeData.lon, s.Latitude, s.Longitude))
                    .FirstOrDefault();

                Station entryStation = stationsArrivee
                    .OrderBy(s => GeoUtils.GetDistance(departData.lat, departData.lon, s.Latitude, s.Longitude))
                    .FirstOrDefault();

                if (exitStation == null || entryStation == null)
                {
                    return new RouteLeg(walkData.duration, $"[Solution : Marche 🚶] (Erreur stations)\nTemps total : {walkData.duration / 60:F1} min.", walkData.geometry);
                }

                // --- CALCUL DES 3 SEGMENTS (AVEC GÉOMÉTRIE) ---

                // Segment 1 : Départ -> Sortie
                RouteLeg leg1 = await CalculateBestTimeInContractAsync(
                    (departData.lon, departData.lat),
                    (exitStation.Longitude, exitStation.Latitude),
                    stationsDepart, "Départ", $"Sortie {contractDepart}");

                // Segment 2 : Marche Inter-Villes (UTILISER GetRouteDataAsync !)
                var leg2Data = await _orsService.GetRouteDataAsync(
                    (exitStation.Longitude, exitStation.Latitude),
                    (entryStation.Longitude, entryStation.Latitude),
                    "foot-walking");

                // Segment 3 : Entrée -> Arrivée
                RouteLeg leg3 = await CalculateBestTimeInContractAsync(
                    (entryStation.Longitude, entryStation.Latitude),
                    (arriveeData.lon, arriveeData.lat),
                    stationsArrivee, $"Entrée {contractArrivee}", "Arrivée");

                double totalLegalSec = leg1.TotalSeconds + leg2Data.duration + leg3.TotalSeconds;

                // Fusionner les 3 géométries
                List<double[]> fullGeometry = new List<double[]>();
                fullGeometry.AddRange(leg1.Geometry);
                fullGeometry.AddRange(leg2Data.geometry);
                fullGeometry.AddRange(leg3.Geometry);

                if (totalLegalSec < walkData.duration)
                {
                    string result = $"[Solution : Combiné Légal 🚴‍♂️🚶🚴‍♂️] (Temps total: {totalLegalSec / 60:F1} min)\n";
                    result += "--- Partie 1 ---\n" + leg1.Description + "\n";
                    result += $"--- Partie 2 ---\nMarche inter-villes ({leg2Data.duration / 60:F1} min)\n";
                    result += "--- Partie 3 ---\n" + leg3.Description;

                    return new RouteLeg(totalLegalSec, result, fullGeometry);
                }
                else
                {
                    string msg = $"[Solution : Marche 🚶] (Plus rapide)\nTemps total : {walkData.duration / 60:F1} min.";
                    return new RouteLeg(walkData.duration, msg, walkData.geometry);
                }
            }
        }

        // (DANS ItineraryService.cs)

        private async Task<RouteLeg> CalculateBestTimeInContractAsync(
            (double lon, double lat) startPoint,
            (double lon, double lat) endPoint,
            List<Station> stationsInContract,
            string startLocationName = "Point de départ",
            string endLocationName = "Destination")
        {
            // 1. Calculer la référence : 100% à pied (AVEC GÉOMÉTRIE)
            var walkData = await _orsService.GetRouteDataAsync(startPoint, endPoint, "foot-walking");
            double walkingDurationSec = walkData.duration;

            // Cas "Marche obligatoire" (pas de stations ou erreur)
            if (stationsInContract == null || stationsInContract.Count == 0)
            {
                string desc = $"[Marche 🚶] (Pas de stations) Temps total : {walkingDurationSec / 60:F1} minutes.";
                return new RouteLeg(walkingDurationSec, desc, walkData.geometry);
            }

            // 2. Trouver les stations (Identique à avant)
            Station startStation = stationsInContract
                .Where(s => s.AvailableBikes > 0)
                .OrderBy(s => GeoUtils.GetDistance(startPoint.lat, startPoint.lon, s.Latitude, s.Longitude))
                .FirstOrDefault();

            Station endStation = stationsInContract
                .Where(s => s.AvailableBikeStands > 0)
                .OrderBy(s => GeoUtils.GetDistance(endPoint.lat, endPoint.lon, s.Latitude, s.Longitude))
                .FirstOrDefault();

            if (startStation == null || endStation == null)
            {
                string desc = $"[Marche 🚶] (Pas de vélos/places) Temps total : {walkingDurationSec / 60:F1} minutes.";
                return new RouteLeg(walkingDurationSec, desc, walkData.geometry);
            }

            // 3. Calculer le temps total à vélo (3 segments AVEC GÉOMÉTRIE)

            // Marche 1
            var leg1 = await _orsService.GetRouteDataAsync(
                startPoint, (startStation.Longitude, startStation.Latitude), "foot-walking");

            // Vélo
            var leg2 = await _orsService.GetRouteDataAsync(
                (startStation.Longitude, startStation.Latitude), (endStation.Longitude, endStation.Latitude), "cycling-regular");

            // Marche 2
            var leg3 = await _orsService.GetRouteDataAsync(
                (endStation.Longitude, endStation.Latitude), endPoint, "foot-walking");

            double bikeTotalDurationSec = leg1.duration + leg2.duration + leg3.duration;

            // 4. Fusionner les géométries (Mettre les listes bout à bout)
            List<double[]> fullBikeGeometry = new List<double[]>();
            fullBikeGeometry.AddRange(leg1.geometry);
            fullBikeGeometry.AddRange(leg2.geometry);
            fullBikeGeometry.AddRange(leg3.geometry);

            // 5. Comparer
            double walkingMinutes = walkingDurationSec / 60;
            double bikeMinutes = bikeTotalDurationSec / 60;

            if (bikeTotalDurationSec < walkingDurationSec)
            {
                // Vélo gagne
                string desc = $"[Vélo 🚴‍♂️] (Temps total: {bikeMinutes:F1} min)\n";
                desc += $"1. Marchez ({startLocationName} -> '{startStation.Name}') ({leg1.duration / 60:F1} min)\n";
                desc += $"2. Roulez ('{startStation.Name}' -> '{endStation.Name}') ({leg2.duration / 60:F1} min)\n";
                desc += $"3. Marchez ('{endStation.Name}' -> {endLocationName}) ({leg3.duration / 60:F1} min)\n";
                desc += $"(Alternative à pied : {walkingMinutes:F1} min)";

                return new RouteLeg(bikeTotalDurationSec, desc, fullBikeGeometry);
            }
            else
            {
                // Marche gagne
                string desc = $"[Marche 🚶] ({startLocationName} -> {endLocationName}) (Temps total: {walkingMinutes:F1} min)\n";
                desc += $"(L'itinéraire à vélo prendrait {bikeMinutes:F1} min)";

                return new RouteLeg(walkingDurationSec, desc, walkData.geometry);
            }
        }

        // (DANS LA CLASSE ItineraryService)

        /// <summary>
        /// Ancienne méthode, renvoie juste la description.
        /// Appelle la nouvelle "super-fonction".
        /// </summary>
        private async Task<string> FindBestRouteInContractAsync(
            (double lon, double lat) startPoint,
            (double lon, double lat) endPoint,
            List<Station> stationsInContract)
        {
            // On appelle la nouvelle fonction et on ne retourne que la partie "Description"
            RouteLeg leg = await CalculateBestTimeInContractAsync(
                startPoint,
                endPoint,
                stationsInContract,
                "Point de départ", // On garde les noms génériques
                "Destination");

            return leg.Description;
        }
    }
}