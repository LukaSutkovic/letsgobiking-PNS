using System;
using System.Collections.Generic;
using System.Net.Http;
using System.ServiceModel.Web;
using System.Threading.Tasks;
using ProxyCore.Dto;

namespace ProxyCore
{
    public sealed class ProxyService : IProxy, IDisposable
    {
        private readonly string _apiKey;
        private readonly HttpClient _http;
        private readonly GenericProxyCache _cache = new GenericProxyCache();

        public ProxyService()
        {
            _apiKey = System.Configuration.ConfigurationManager.AppSettings["JCDECAUX_API_KEY"];
            if (string.IsNullOrWhiteSpace(_apiKey))
                throw new InvalidOperationException("JCDECAUX_API_KEY manquante.");

            _http = new HttpClient { BaseAddress = new Uri("https://api.jcdecaux.com/vls/v3/") };
            _http.Timeout = TimeSpan.FromSeconds(15);
        }

        public Task<List<Contract>> GetContracts() =>
            _cache.GetOrCreateAsync(
                "contracts",
                TimeSpan.FromHours(12),
                () => HttpJson.GetAsync<List<Contract>>(_http, $"contracts?apiKey={Uri.EscapeDataString(_apiKey)}")
            );

        public Task<List<Station>> GetStations(string contract)
        {
            if (string.IsNullOrWhiteSpace(contract))
                throw new WebFaultException<string>("Missing contract", System.Net.HttpStatusCode.BadRequest);

            var key = $"stations::{contract.ToLowerInvariant()}";
            return _cache.GetOrCreateAsync(
                key,
                TimeSpan.FromMinutes(1),
                () => HttpJson.GetAsync<List<Station>>(
                    _http, $"stations?contract={Uri.EscapeDataString(contract)}&apiKey={Uri.EscapeDataString(_apiKey)}")
            );
        }

        public void Dispose() => _http?.Dispose();
    }
}
