using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using ProxyCore;

namespace ProxyHost
{
    internal static class Program
    {
        static void Main()
        {
            var baseAddress = new Uri("http://localhost:9001/api/");
            using (var host = new WebServiceHost(typeof(ProxyService), baseAddress))
            {
                var ep = host.AddServiceEndpoint(typeof(IProxy), new WebHttpBinding(), "");
                ep.EndpointBehaviors.Add(new WebHttpBehavior
                {
                    AutomaticFormatSelectionEnabled = true,
                    DefaultOutgoingResponseFormat = WebMessageFormat.Json
                });

                // optionnel: désactiver la help page WCF
                var dbg = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                if (dbg != null) dbg.HttpHelpPageEnabled = false;

                host.Open();
                Console.WriteLine("Proxy up: http://localhost:9001/api");
                Console.WriteLine("GET /contracts");
                Console.WriteLine("GET /stations?contract={name}");
                Console.WriteLine("Press ENTER to stop.");
                Console.ReadLine();
                host.Close();
            }
        }
    }
}
