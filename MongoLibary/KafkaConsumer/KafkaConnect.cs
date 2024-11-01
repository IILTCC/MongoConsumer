using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoLibary.KafkaConsumer
{
    class KafkaConnect
    {
        public KafkaConnect()
        {

        }
        public void WaitForKafkaConnection()
        {
            IAdminClient _adminClient;

            //TODO: add app settings
            AdminClientConfig adminConfig = new AdminClientConfig
            {
                BootstrapServers = "localhost:9092"
            };
            _adminClient = new AdminClientBuilder(adminConfig).Build();
            while (true)
            {
                try
                {
                    _adminClient.GetMetadata(TimeSpan.FromSeconds(5));
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


        public (IConsumer<Ignore, string> consumer, CancellationTokenSource cancelToken) ProvideConsumer(List<string> topicNames)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = "my-consumer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            IConsumer<Ignore, string> consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe(topicNames);

            CancellationTokenSource cts = new CancellationTokenSource();
            // cancel on console ctrl c
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };
            return (consumer, cts);
        }
    }
}
