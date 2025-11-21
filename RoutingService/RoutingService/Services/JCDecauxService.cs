using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RoutingService.Services
{
    // === DTO ALIGNÉ SUR LE JSON DU PROXY ===

    public class PositionDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class AvailabilitiesDto
    {
        public int Bikes { get; set; }
        public int Stands { get; set; }
    }

    public class TotalStandsDto
    {
        public AvailabilitiesDto Availabilities { get; set; } = new();
    }

    public class ProxyStationDto
    {
        public string Address { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Number { get; set; }

        public PositionDto Position { get; set; } = new();
        public TotalStandsDto TotalStands { get; set; } = new();
    }

    public class Station
    {
        public int Number { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int AvailableBikes { get; set; }
        public int AvailableBikeStands { get; set; }
    }

    public class JCDecauxService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:9001/api/")
        };

        public async Task<List<Station>> GetStationsAsync(string contractName)
        {
            Console.WriteLine($"[JCDecauxService] contractName = '{contractName ?? "NULL"}'");

            if (string.IsNullOrWhiteSpace(contractName))
            {
                Console.WriteLine("[JCDecauxService] contractName null ou vide, on n'appelle PAS le proxy.");
                return new List<Station>();
            }

            string safeContract = Uri.EscapeDataString(contractName);
            string requestUrl = $"stations?contract={safeContract}";

            Console.WriteLine($"[JCDecauxService] Appel proxy : {_httpClient.BaseAddress}{requestUrl}");

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                Console.WriteLine($"[JCDecauxService] StatusCode proxy = {(int)response.StatusCode} {response.StatusCode}");

                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();

                Console.WriteLine("========== [JCDecauxService] JSON du proxy (début) ==========");
                Console.WriteLine(jsonResponse.Substring(0, Math.Min(jsonResponse.Length, 500)));
                Console.WriteLine("==============================================================");

                // Désérialisation DIRECTE du JSON venant du proxy
                var proxyStations = JsonConvert.DeserializeObject<List<ProxyStationDto>>(jsonResponse);

                if (proxyStations == null)
                {
                    Console.WriteLine("[JCDecauxService] Désérialisation retourne null.");
                    return new List<Station>();
                }

                var stationsList = new List<Station>();

                foreach (var ps in proxyStations)
                {
                    var station = new Station
                    {
                        Number = ps.Number,
                        Name = ps.Name ?? string.Empty,
                        Latitude = ps.Position?.Latitude ?? 0.0,
                        Longitude = ps.Position?.Longitude ?? 0.0,
                        AvailableBikes = ps.TotalStands?.Availabilities?.Bikes ?? 0,
                        AvailableBikeStands = ps.TotalStands?.Availabilities?.Stands ?? 0
                    };

                    stationsList.Add(station);
                }

                Console.WriteLine($"[JCDecauxService] {stationsList.Count} stations reçues pour '{contractName}'.");

                return stationsList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("[JCDecauxService] Erreur JCDecaux ntzth zertherghbem:");
                Console.WriteLine(ex.ToString());   // stack trace complète
                return new List<Station>();
            }
        }
    }
}
