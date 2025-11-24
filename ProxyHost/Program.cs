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
<<<<<<< HEAD
               
=======
                // On configure le binding manuellement
>>>>>>> e51141041042421f64460e232c51f505375363c0
                var binding = new WebHttpBinding();

                binding.HostNameComparisonMode = HostNameComparisonMode.Exact;

                binding.Security.Mode = WebHttpSecurityMode.None;
                binding.CrossDomainScriptAccessEnabled = true;

                var ep = host.AddServiceEndpoint(typeof(IProxy), binding, "");

                

                ep.EndpointBehaviors.Add(new WebHttpBehavior
                {
                    AutomaticFormatSelectionEnabled = true,
                    DefaultOutgoingResponseFormat = WebMessageFormat.Json
                });

                // optionnel: désactiver la help page WCF
                var dbg = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                if (dbg != null) dbg.HttpHelpPageEnabled = false;

                try
                {
                    host.Open();
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("Proxy demarre avec succes");
                    Console.WriteLine($"URL : {baseAddress}");
                    Console.WriteLine("--------------------------------------------------");
                    Console.WriteLine("GET /contracts");
                    Console.WriteLine("GET /stations?contract={name}");
                    Console.WriteLine("Appuyez sur ENTREE pour arreter.");
                    Console.ReadLine();
                    host.Close();
                }
                catch (AddressAccessDeniedException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERREUR : Accès refusé.");
                    Console.WriteLine("Windows bloque encore le port.");
                    Console.WriteLine("Essayez de changer le port.");
                    Console.ResetColor();
                    Console.ReadLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur fatale : {ex.Message}");
                    Console.ReadLine();
                }
            }
        }
    }
}