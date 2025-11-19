using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RoutingService.Services
{
    public class Station
    {
        public int Number { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AvailableBikes { get; set; }
        public int AvailableBikeStands { get; set; }
    }

    public class JCDecauxService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:9001/api/") // PROXY
        };

        public async Task<List<Station>> GetStationsAsync(string contractName)
        {
            Console.WriteLine($"[JCDecauxService] contractName = '{contractName ?? "NULL"}'");

            if (string.IsNullOrWhiteSpace(contractName))
            {
                Console.WriteLine("[JCDecauxService] contractName null ou vide, on n'appelle PAS le proxy.");
                return new List<Station>();
            }

            // IMPORTANT : on utilise BaseAddress + chemin relatif
            string safeContract = Uri.EscapeDataString(contractName);
            string requestUrl = $"stations?contract={safeContract}";

            Console.WriteLine($"[JCDecauxService] Appel proxy : { _httpClient.BaseAddress }{ requestUrl }");

            var stationsList = new List<Station>();

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var stations = JArray.Parse(jsonResponse);
                foreach (JObject stationData in stations)
                {
                    var station = new Station
                    {
                        Number = (int)stationData["number"],
                        Name = (string)stationData["name"],
                        Latitude = (double)stationData["position"]["latitude"],
                        Longitude = (double)stationData["position"]["longitude"],
                        AvailableBikes = (int)stationData["totalStands"]["availabilities"]["bikes"],
                        AvailableBikeStands = (int)stationData["totalStands"]["availabilities"]["stands"]
                    };

                    stationsList.Add(station);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur JCDecaux : {ex.Message}");
            }

            return stationsList;
        }
    }
}
