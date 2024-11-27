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
            IAdminClient adminClient = InitializeAdminClient();
            while (true)
            {
                try
                {
                    adminClient.GetMetadata(TimeSpan.FromSeconds(Consts.TIMEOUT));
                    return;
                }
                catch (KafkaException e)
                {
                }
                catch (Exception e)
                {
                }
            }
        }

        public IAdminClient InitializeAdminClient()
        {
            AdminClientConfig adminConfig = new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.KafkaUrl
            };
            IAdminClient adminClient = new AdminClientBuilder(adminConfig).Build();
            return adminClient;
        }

        public CancellationToken CancellationToken(IConsumer<Ignore,string> consumer)
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
        public IConsumer<Ignore, string> Consumer(List<string> topicNames)
        {
            ConsumerConfig config = new ConsumerConfig
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
