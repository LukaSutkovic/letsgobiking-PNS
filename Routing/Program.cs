using Routing;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace Routing
{
    internal static class Program
    {
        static void Main()
        {
            var baseAddress = new Uri("http://localhost:9002/api");
            using (var host = new WebServiceHost(typeof(RoutingService), baseAddress))
            {
                var ep = host.AddServiceEndpoint(typeof(IRouting), new WebHttpBinding(), "");
                ep.EndpointBehaviors.Add(new WebHttpBehavior { HelpEnabled = true });

                host.Open();
                Console.WriteLine("Routing up: " + baseAddress);
                Console.WriteLine("GET /route?contract={name}&olat={...}&olon={...}&dlat={...}&dlon={...}");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}
