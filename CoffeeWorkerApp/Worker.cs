using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CoffeeWorkerApp.Config;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CoffeeWorkerApp
{
    public class Worker : BackgroundService
    {
        const int commitPeriod = 5;
        private readonly ILogger<Worker> _logger;
        private readonly KafkaConsumerConfiguration _config;

        private readonly ConsumerConfig _consumerConfig;

        public Worker (ILogger<Worker> logger, IOptions<KafkaConsumerConfiguration> config)
        {
            _logger = logger;
            _config = config.Value;

            _consumerConfig = new ConsumerConfig ()
            {
                BootstrapServers = _config.BootStrapServers,
                GroupId = _config.GroupId,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslUsername = _config.SaslUsername,
                SaslPassword = _config.SaslPasword,
                ApiVersionRequest = true,
                SaslMechanism = SaslMechanism.ScramSha256,
            };
        }

        protected override async Task ExecuteAsync (CancellationToken stoppingToken)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string> (_consumerConfig)
                // Note: All handlers are called on the main .Consume thread.
                .SetErrorHandler ((_, e) => Console.WriteLine ($"Error: {e.Reason}"))
                .SetStatisticsHandler ((_, json) => Console.WriteLine ($"Statistics: {json}"))
                .SetPartitionsAssignedHandler ((c, partitions) =>
                {
                    _logger.LogInformation ($"Assigned partitions: [{string.Join(", ", partitions)}]");
                    //Console.WriteLine($"Assigned partitions: [{string.Join(", ", partitions)}]");

                    // possibly manually specify start offsets or override the partition assignment provided by
                    // the consumer group by returning a list of topic/partition/offsets to assign to, e.g.:
                    // 
                    // return partitions.Select(tp => new TopicPartitionOffset(tp, externalOffsets[tp]));
                })
                .SetPartitionsRevokedHandler ((c, partitions) =>
                {
                    _logger.LogInformation ($"Revoking assignment: [{string.Join(", ", partitions)}]");
                    //Console.WriteLine($"Revoking assignment: [{string.Join(", ", partitions)}]");
                })
                .Build ())
            {
                consumer.Subscribe (new List<string> () { _config.Topic });

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var consumeResult = consumer.Consume ();

                            if (consumeResult.IsPartitionEOF)
                            {
                                _logger.LogInformation ($"Reached end of topic {consumeResult.Topic}, partition {consumeResult.Partition}, offset {consumeResult.Offset}.");

                                continue;
                            }

                            _logger.LogInformation ($"Received message at {consumeResult.TopicPartitionOffset}: {consumeResult.Value}");

                            if (consumeResult.Offset % commitPeriod == 0)
                            {
                                // The Commit method sends a "commit offsets" request to the Kafka
                                // cluster and synchronously waits for the response. This is very
                                // slow compared to the rate at which the consumer is capable of
                                // consuming messages. A high performance application will typically
                                // commit offsets relatively infrequently and be designed handle
                                // duplicate messages in the event of failure.
                                try
                                {
                                    consumer.Commit (consumeResult);
                                }
                                catch (KafkaException e)
                                {
                                    _logger.LogError (e, $"Commit error: {e.Error.Reason}");
                                }
                            }
                        }
                        catch (ConsumeException e)
                        {
                            _logger.LogError (e, $"Commit error: {e.Error.Reason}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation ("Closing consumer.");
                    consumer.Close ();
                }
            }
        }
    }
}