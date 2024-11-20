using Confluent.Kafka;
using MongoConsumer.AppSettings;
using MongoConsumerLibary.KafkaConsumer;
using MongoConsumerLibary.MongoConnection;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoConsumer
{
    class Startup
    {
        private readonly KafkaSettings _kafkaSettings;
        private readonly KafkaConnection _kafkaConnection;
        private readonly ZlibCompression _zlibCompression;
        private readonly MongoConnection _mongoConnection;
        public Startup() 
        {
            ConfigProvider configProvider = ConfigProvider.Instance;
            _kafkaSettings = configProvider.KafkaSettings();
            _kafkaConnection = new KafkaConnection(_kafkaSettings);
            _zlibCompression = new ZlibCompression();
            _mongoConnection = new MongoConnection(configProvider.MongoSettings());
        }
        public List<string> InitializeTopicNames()
        {
            List<string> topicNames = new List<string>();
            foreach (string topic in _kafkaSettings.KafkaTopics)
                topicNames.Add(topic);
            
            return topicNames;
        }
        public void StartMongoConsumer()
        {
            _kafkaConnection.WaitForKafkaConnection();
            IConsumer<Ignore, string> consumer = _kafkaConnection.Consumer(InitializeTopicNames());
            CancellationToken cancellationToken =  _kafkaConnection.CancellationToken(consumer);
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
