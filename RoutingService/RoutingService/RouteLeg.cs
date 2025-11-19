using System.Collections.Generic;

namespace RoutingService.Services
{
    /// <summary>
    /// Représente une "étape" (Leg) d'un itinéraire.
    /// Contient le temps, la description ET la géométrie (les points GPS).
    /// </summary>
    public class RouteLeg
    {
        public double TotalSeconds { get; set; }
        public string Description { get; set; }

        // NOUVEAU : La liste des points GPS pour tracer la ligne
        // Chaque point est un tableau de 2 doubles : [longitude, latitude]
        public List<double[]> Geometry { get; set; }

        public RouteLeg(double totalSeconds, string description, List<double[]> geometry = null)
        {
            TotalSeconds = totalSeconds;
            Description = description;
            Geometry = geometry ?? new List<double[]>(); // Si null, on met une liste vide
        }
    }
}