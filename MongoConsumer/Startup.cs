﻿using Confluent.Kafka;
using MongoConsumer.AppSettings;
using MongoConsumerLibary.KafkaConsumer;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MongoConsumer
{
    class Startup
    {
        private readonly KafkaSettings _kafkaSettings;
        private readonly KafkaConnection _kafkaConnection;
        public Startup() 
        {
            SettingsProvider settingsProvider = SettingsProvider.Instance;
            _kafkaSettings = settingsProvider.ProvideKafkaSettings();
            _kafkaConnection = new KafkaConnection(_kafkaSettings);
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