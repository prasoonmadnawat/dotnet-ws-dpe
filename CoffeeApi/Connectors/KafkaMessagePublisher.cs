using System.Threading.Tasks;
using CoffeeApi.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace CoffeeApi.Connectors
{
    public class KafkaMessagePublisher : IMessagePublisher
    {
        private readonly KafkaProducerConfiguration _config;
        private readonly ProducerConfig _producerConfig;

        public KafkaMessagePublisher (IOptions<KafkaProducerConfiguration> config)
        {
            this._config = config.Value;
            this._producerConfig = new ProducerConfig ()
            {
                BootstrapServers = _config.BootStrapServers,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = _config.SaslUsername,
                SaslPassword = _config.SaslPasword,
                ApiVersionRequest = true,
                SaslMechanism = SaslMechanism.ScramSha256,
                // Debug = "broker,topic,msg"
            };
        }

        public async Task PublishMessageAsync (string message)
        {
            using (var producer = new ProducerBuilder<Null, string> (this._producerConfig).Build ())
            {
                await producer.ProduceAsync (topic: this._config.Topic, new Message<Null, string> () { Value = message });
                producer.Flush ();
            }
        }
    }
}