using Confluent.Kafka;

namespace CoffeeApi.Config
{
    public class KafkaProducerConfiguration
    {
        public string BootStrapServers { get; set; }

        public string SaslUsername { get; set; }

        public string SaslPasword { get; set; }

        public string Topic { get; set; }
    }
}