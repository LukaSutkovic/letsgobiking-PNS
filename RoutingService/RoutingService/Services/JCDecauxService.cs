using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace RoutingService.Services
{
    public class Station //modele simple pour stocker ce qu'on veut
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
        private static readonly HttpClient _httpClient = new HttpClient();

        
        private readonly string _apiKey = "2cf243f8c91d6e37957db0ca1018835fde99bcfc";

        /// <summary>
        /// Récupère toutes les stations pour un contrat (ville) donné.
        /// </summary>
        public async Task<List<Station>> GetStationsAsync(string contractName)
        {
            
            string requestUrl = $"https://api.jcdecaux.com/vls/v3/stations?contract={contractName}&apiKey={_apiKey}";

            var stationsList = new List<Station>();

            try
            {
                
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                
                string jsonResponse = await response.Content.ReadAsStringAsync();

                
                JArray stations = JArray.Parse(jsonResponse);

                
                foreach (JObject stationData in stations)
                {
                    
                    Station station = new Station//creation station propre
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
                // Si le contrat n'existe pas dans la ville style aubagne
                Console.WriteLine($"Erreur JCDecaux : {ex.Message}");
                
            }

            
            return stationsList;
        }
    }
}