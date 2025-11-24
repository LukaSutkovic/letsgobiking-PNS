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
            Console.WriteLine("[NotifService] Demarrage du service de notifications...");

            // À adapter selon la config ActiveMQ du prof
            string brokerUri = "activemq:tcp://localhost:61616";
            string username = "admin";
            string password = "admin";

            // Topics utilisés
            string[] topics = new[]
            {
                "meteo",
                "pollution",
                "airquality"
            };

            IConnectionFactory factory = new ConnectionFactory(brokerUri);

            using var connection = factory.CreateConnection(username, password);
            connection.Start();

            using var session = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

            // On garde un producteur par topic
            var producers = new (string topicName, IMessageProducer producer)[topics.Length];
            for (int i = 0; i < topics.Length; i++)
            {
                string topicName = topics[i];
                IDestination dest = session.GetTopic(topicName);
                producers[i] = (topicName, session.CreateProducer(dest));
            }

            var rand = new Random();

            while (true)
            {
                // On choisit un topic au hasard
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

                // facultatif : entêtes pour filtrage côté client
                msg.Properties["topic"] = topicName;
                msg.Properties["severity"] = payload.severity;

                producer.Send(msg);

                Console.WriteLine($"[NotifService] Sent to '{topicName}': {json}");

                // On flood un peu pour la démo → tous les 5s
                Thread.Sleep(TimeSpan.FromSeconds(5));
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
                "airquality" => "Qualité de l'air dégradée, évitez l'effort intense",
                _ => "Notification generique"
            };
        }
    }
}
