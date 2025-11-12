using ProxyCore; // pour Contract/Station + HttpJson
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ProxyCore.Dto;

namespace Routing
{
    public sealed class RoutingService : IRouting
    {
        private static readonly string ProxyUrl =
        Environment.GetEnvironmentVariable("PROXY_URL") ?? "http://localhost:9001/api";

        private readonly ProxyClient _proxy = new ProxyClient(ProxyUrl);

        public Task<List<Contract>> Contracts() => _proxy.GetContracts();

        public Task<List<Station>> Stations(string contract) => _proxy.GetStations(contract);
    }
}
