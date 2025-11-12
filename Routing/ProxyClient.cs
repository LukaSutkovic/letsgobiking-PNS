using ProxyCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using ProxyCore.Dto;

namespace Routing
{
    public sealed class ProxyClient : IDisposable
    {
        private readonly ChannelFactory<IProxy> _factory;
        private readonly IProxy _ch;

        public ProxyClient(string baseUrl)
        {
            var binding = new WebHttpBinding();
            var addr = new EndpointAddress(baseUrl.TrimEnd('/') + "/");
            _factory = new ChannelFactory<IProxy>(binding, addr);
            _factory.Endpoint.EndpointBehaviors.Add(new WebHttpBehavior());
            _ch = _factory.CreateChannel();
        }

        public System.Threading.Tasks.Task<System.Collections.Generic.List<Contract>> GetContracts()
            => _ch.GetContracts();

        public System.Threading.Tasks.Task<System.Collections.Generic.List<Station>> GetStations(string contract)
            => _ch.GetStations(contract);

        public void Dispose()
        {
            try { ((IClientChannel)_ch)?.Close(); _factory?.Close(); }
            catch { ((IClientChannel)_ch)?.Abort(); _factory?.Abort(); }
        }
    }
}
