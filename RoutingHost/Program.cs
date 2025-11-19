using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;

namespace RoutingHost
{
    internal static class Program
    {
        static void Main()
        {
            var baseAddr = new Uri("http://localhost:9002/api");
            using (var host = new WebServiceHost(typeof(RouterService), baseAddr))
            {
                var ep = host.AddServiceEndpoint(typeof(IRouter), new WebHttpBinding(), "");
                ep.EndpointBehaviors.Add(new WebHttpBehavior { HelpEnabled = true, AutomaticFormatSelectionEnabled = true });

                host.Open();
                Console.WriteLine("Routing up: " + baseAddr);
                Console.WriteLine("GET /route?origin=43.6167,7.0530&dest=43.7034,7.2663");
                Console.WriteLine("Press Ctrl+C to exit.");
                Console.ReadLine();
            }
        }
    }
}
