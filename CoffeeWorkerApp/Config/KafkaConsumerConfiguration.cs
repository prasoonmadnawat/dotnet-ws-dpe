namespace CoffeeWorkerApp.Config
{
    public class KafkaConsumerConfiguration
    {
        public string BootStrapServers { get; set; }

        public string GroupId { get; set; }

        public string SaslUsername { get; set; }

        public string SaslPasword { get; set; }

        public string Topic { get; set; }
    }
}