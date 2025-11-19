using System;

namespace RoutingService.Utils
{
    public static class GeoUtils
    {
        
        private const double EarthRadiusKm = 6371.0;

        /// <summary>
        /// Calcule la distance (en kilomètres) entre deux points GPS
        /// en utilisant la formule de Haversine.
        /// </summary>
        public static double GetDistance(
            double startLat, double startLon,
            double endLat, double endLon)
        {
            // Conversion des degrés en radians
            double dLat = ToRadians(endLat - startLat);
            double dLon = ToRadians(endLon - startLon);
            double lat1 = ToRadians(startLat);
            double lat2 = ToRadians(endLat);

            // Formule de Haversine
            double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                       Math.Pow(Math.Sin(dLon / 2), 2) *
                       Math.Cos(lat1) * Math.Cos(lat2);

            double c = 2 * Math.Asin(Math.Sqrt(a));

            return EarthRadiusKm * c;
        }

        
        private static double ToRadians(double angleInDegrees)
        {
            return (Math.PI / 180) * angleInDegrees;
        }
    }
}