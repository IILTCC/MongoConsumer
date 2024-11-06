using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoConsumerLibary.KafkaConsumer
{
    public class KafkaConnection
    {
        private readonly KafkaSettings _kafkaSettings;
        public KafkaConnection(KafkaSettings kafkaSettings)
        {
            _kafkaSettings = kafkaSettings;
        }

        public void WaitForKafkaConnection()
        {
            const int TIMEOUT = 5;
            IAdminClient _adminClient;

            AdminClientConfig adminConfig = new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.KafkaUrl
            };
            _adminClient = new AdminClientBuilder(adminConfig).Build();
            while (true)
            {
                try
                {
                    _adminClient.GetMetadata(TimeSpan.FromSeconds(TIMEOUT));
                    return;
                }
                catch (KafkaException e)
                {
                    Console.WriteLine(e);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
        public CancellationToken ProvideCancellationToken(IConsumer<Ignore,string> consumer)
        {
            CancellationTokenSource cts = new CancellationTokenSource();

            // cancel on console ctrl c
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                consumer.Close();  // Commit offsets and unsubscribe before shutting down
                cts.Cancel();
            };
            CancellationToken cancelToken = cts.Token;
            return cancelToken;
        }
        public IConsumer<Ignore, string> ProvideConsumer(List<string> topicNames)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _kafkaSettings.KafkaUrl,
                GroupId = _kafkaSettings.KafkaConsumerGroup,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            IConsumer<Ignore, string> consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topicNames);

            return consumer;
        }
    }
}
