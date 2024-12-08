﻿using Confluent.Kafka;
using MongoConsumer.AppSettings;
using MongoConsumerLibary.KafkaConsumer;
using MongoConsumerLibary.MongoConnection;
using System;
using System.Collections.Generic;
using System.Threading;
using MongoConsumerLibary;
using MongoConsumerLibary.MongoConnection.Collections;
using MongoConsumer.Logs;

namespace MongoConsumer
{
    class Startup
    {
        private readonly KafkaSettings _kafkaSettings;
        private readonly KafkaConnection _kafkaConnection;
        private readonly ZlibCompression _zlibCompression;
        private readonly MongoConnection _mongoConnection;
        private readonly MongoSettings _mongoSettings;
        private readonly MongoConsumerLogger _logger;
        public Startup() 
        {
            ConfigProvider configProvider = ConfigProvider.Instance;
            _logger = MongoConsumerLogger.Instance;
            _kafkaSettings = configProvider.ProvideKafkaSettings();
            _mongoSettings = configProvider.ProvideMongoSettings();
            _zlibCompression = new ZlibCompression();

            _logger.LogInfo("Waiting for kafka connection");
            _kafkaConnection = new KafkaConnection(_kafkaSettings);
            _logger.LogInfo("Connected to kafka");

            _logger.LogInfo("waiting for mongo connection");
            _mongoConnection = new MongoConnection(configProvider.ProvideMongoSettings());
            _logger.LogInfo("Connected to mongo");

            _logger.LogInfo("Succesfuly initated mongo consumer");
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
            CancellationToken cancellationToken = _kafkaConnection.CancellationToken(consumer);
            _logger.LogInfo("Started mongo consumer");
            while (true)
            {
                try
                {
                    ConsumeResult<Ignore,string> consumerResult = consumer.Consume(cancellationToken);
                    if (consumerResult.Topic != Consts.TELEMETRY_TOPIC_NAME)
                    {
                        BaseBoxCollection baseBoxCollection = new BaseBoxCollection();
                        baseBoxCollection.CompressedData = _zlibCompression.CompressData(consumerResult.Message.Value);
                        baseBoxCollection.IcdType = consumerResult.Topic;
                        _mongoConnection.AddDocument(baseBoxCollection,_mongoSettings.DocumentTTL);
                    }
                }
                catch(KafkaException e)
                {
                    _logger.LogFatal("Tried receive data from kafka - "+e.Message);
                }
                catch(Exception e)
                {
                    _logger.LogFatal("Tried receive data from kafka -"+e.Message);
                }
            }
        }
    }
}
