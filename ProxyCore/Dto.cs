using Newtonsoft.Json;

namespace ProxyCore.Dto
{
    public sealed class Contract
    {
        [Newtonsoft.Json.JsonProperty("name")] public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty("commercial_name")] public string CommercialName { get; set; }
        [Newtonsoft.Json.JsonProperty("cities")] public System.Collections.Generic.List<string> Cities { get; set; }
        [Newtonsoft.Json.JsonProperty("country_code")] public string CountryCode { get; set; }
    }

    public sealed class Position
    {
        [Newtonsoft.Json.JsonProperty("latitude")] public double Latitude { get; set; }
        [Newtonsoft.Json.JsonProperty("longitude")] public double Longitude { get; set; }
    }

    public sealed class Availabilities
    {
        [Newtonsoft.Json.JsonProperty("bikes")] public int Bikes { get; set; }
        [Newtonsoft.Json.JsonProperty("stands")] public int Stands { get; set; }
    }

    public sealed class TotalStands
    {
        [Newtonsoft.Json.JsonProperty("availabilities")] public Availabilities Availabilities { get; set; }
    }

    public sealed class Station
    {
        [Newtonsoft.Json.JsonProperty("number")] public int Number { get; set; }
        [Newtonsoft.Json.JsonProperty("name")] public string Name { get; set; }
        [Newtonsoft.Json.JsonProperty("address")] public string Address { get; set; }
        [Newtonsoft.Json.JsonProperty("position")] public Position Position { get; set; }
        [Newtonsoft.Json.JsonProperty("totalStands")] public TotalStands TotalStands { get; set; }
    }
}

