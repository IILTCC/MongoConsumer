using Confluent.Kafka;
using MongoConsumer.AppSettings;
using MongoConsumerLibary.KafkaConsumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MongoConsumer
{
    class Startup
    {
        private readonly KafkaSettings _kafkaSettings;
        private readonly KafkaConnection _kafkaConnection;
        public Startup(KafkaSettings kafkaSettings) 
        {
            _kafkaSettings = kafkaSettings;
            _kafkaConnection = new KafkaConnection(_kafkaSettings);
        }
        public List<string> InitializeTopicNames()
        {
            string[] topicTemp = new string[5] { "FlightBoxDownIcd", "FlightBoxUpIcd", "FiberBoxUpIcd", "FiberBoxDownIcd", "TelemetryStatistics" };
            List<string> topicNames = new List<string>(); ;
            foreach (string topic in topicTemp)
                topicNames.Add(topic);
            return topicNames;
        }
        public void StartMongoConsumer()
        {
            _kafkaConnection.WaitForKafkaConnection();
            IConsumer<Ignore, string> consumer = _kafkaConnection.ProvideConsumer(InitializeTopicNames());
            CancellationToken cancellationToken =  _kafkaConnection.ProvideCancellationToken(consumer);
            while(true)
            {
                try
                {
                    ConsumeResult<Ignore,string> consumerResult = consumer.Consume(cancellationToken);
                }
                catch(KafkaException e)
                {
                }
                catch(Exception e)
                {
                }
            }
        }
    }
}
