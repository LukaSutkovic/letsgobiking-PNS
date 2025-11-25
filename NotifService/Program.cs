using System;
using System.Text.Json;
using System.Threading;
using Apache.NMS;
using Apache.NMS.ActiveMQ;

namespace NotifService
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("[NotifService] Démarrage du service de notifications...");

            // Utilisation de 127.0.0.1 pour éviter les problèmes IPv6
            string brokerUri = "tcp://127.0.0.1:61616";
            string username = "admin";
            string password = "admin";

            IConnectionFactory factory = new ConnectionFactory(brokerUri);
            IConnection connection = null;

            // --- BOUCLE DE TENTATIVE DE CONNEXION (RETRY POLICY) ---
            while (true)
            {
                try
                {
                    Console.Write("Tentative de connexion à ActiveMQ... ");
                    connection = factory.CreateConnection(username, password);
                    connection.Start();
                    Console.WriteLine("✅ CONNECTÉ !");
                    break; // On sort de la boucle si ça marche
                }
                catch (Exception)
                {
                    Console.WriteLine("❌ Échec.");
                    Console.WriteLine("ActiveMQ n'est pas encore prêt. Nouvelle tentative dans 3 secondes...");
                    Thread.Sleep(3000);
                }
            }
            // -------------------------------------------------------

            string[] topics = new[] { "meteo", "pollution", "airquality" };

            using ISession session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

            var producers = new (string topicName, IMessageProducer producer)[topics.Length];
            for (int i = 0; i < topics.Length; i++)
            {
                string topicName = topics[i];
                IDestination dest = session.GetTopic(topicName);
                producers[i] = (topicName, session.CreateProducer(dest));
            }

            var rand = new Random();

            Console.WriteLine("Envoi des notifications en cours (Ctrl+C pour arrêter)...");

            while (true)
            {
                try 
                {
                    var (topicName, producer) = producers[rand.Next(producers.Length)];

                    var payload = new
                    {
                        topic = topicName,
                        severity = PickSeverity(rand),
                        message = BuildMessage(topicName, rand),
                        timestamp = DateTime.UtcNow.ToString("O")
                    };

                    string json = JsonSerializer.Serialize(payload);
                    ITextMessage msg = session.CreateTextMessage(json);

                    msg.Properties["topic"] = topicName;
                    msg.Properties["severity"] = payload.severity;

                    producer.Send(msg);

                    Console.WriteLine($"[>>] Sent to '{topicName}': {payload.severity}");
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur lors de l'envoi : {ex.Message}");
                    // Si la connexion est perdue, on pourrait vouloir redémarrer la boucle de connexion ici
                    break; 
                }
            }
        }

        static string PickSeverity(Random rand)
        {
            return rand.Next(3) switch
            {
                0 => "LOW",
                1 => "MEDIUM",
                _ => "HIGH"
            };
        }

        static string BuildMessage(string topic, Random rand)
        {
            return topic switch
            {
                "meteo" => "Averse prévue dans les 30 prochaines minutes",
                "pollution" => "Pic de pollution dans la zone actuelle",
                "airquality" => "Qualité de l'air dégradée",
                _ => "Notification generique"
            };
        }
    }
}