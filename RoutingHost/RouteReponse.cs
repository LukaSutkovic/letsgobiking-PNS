using System.Collections.Generic;

namespace RoutingHost
{
    public sealed class RouteResponse
    {
        public bool usingBike { get; set; }
        public double totalDistanceMeters { get; set; }
        public double totalDurationSeconds { get; set; }

        public Leg walkingToPickup { get; set; }
        public Leg bikeLeg { get; set; }
        public Leg walkingToDest { get; set; }

        public StationRef pickupStation { get; set; }
        public StationRef dropStation { get; set; }
        public string rationale { get; set; }

        public sealed class Leg
        {
            public double distanceMeters { get; set; }
            public double durationSeconds { get; set; }
            public List<string> instructions { get; set; }
        }
        public sealed class StationRef
        {
            public int number { get; set; }
            public string name { get; set; }
            public double lat { get; set; }
            public double lon { get; set; }
        }
    }
}
