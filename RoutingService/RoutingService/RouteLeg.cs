using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RoutingService.Services
{
    [DataContract]
    public class RouteLeg
    {
        [DataMember]
        public double TotalSeconds { get; set; }

        [DataMember]
        public string Description { get; set; }

        // CHANGEMENT ICI : On utilise un tableau de tableaux (double[][])
        // C'est beaucoup plus robuste pour la sérialisation SOAP qu'une List<double[]>
        [DataMember]
        public double[][] Geometry { get; set; }

        public RouteLeg() { }

        public RouteLeg(double totalSeconds, string description, double[][] geometry = null)
        {
            TotalSeconds = totalSeconds;
            Description = description;
            Geometry = geometry ?? new double[0][];
        }
    }
}