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
            // 1. On définit l'adresse (localhost est plus propre avec le mode Exact)
            var baseAddress = new Uri("http://localhost:9001/api/");

            using (var host = new WebServiceHost(typeof(ProxyService), baseAddress))
            {
                // On configure le binding manuellement
                var binding = new WebHttpBinding();

                // C'est CETTE ligne qui permet d'éviter les droits d'administrateur.
                // Elle dit à WCF de n'écouter QUE localhost et de ne pas essayer de réserver "+"
                binding.HostNameComparisonMode = HostNameComparisonMode.Exact;

                // On s'assure qu'il n'y a pas de sécurité complexe qui bloque (HTTP simple)
                binding.Security.Mode = WebHttpSecurityMode.None;
                binding.CrossDomainScriptAccessEnabled = true; // Utile pour les appels JS

                // On applique ce binding spécifique
                var ep = host.AddServiceEndpoint(typeof(IProxy), binding, "");

                // ==============================================================

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
                    Console.WriteLine("Proxy demarre avec succes (Mode Sans Admin)");
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