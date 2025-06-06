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
using HealthCheck;
using System.Threading.Tasks;
using System.Globalization;

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
        private readonly HealthCheckEndPoint _healthCheck;

        public Startup() 
        {
            ConfigProvider configProvider = ConfigProvider.Instance;
            _healthCheck = new HealthCheckEndPoint();
            Task.Run(()=> { _healthCheck.StartUp(configProvider.ProvideHealthCheckSettings()); });
            _logger = MongoConsumerLogger.Instance;
            _kafkaSettings = configProvider.ProvideKafkaSettings();
            _mongoSettings = configProvider.ProvideMongoSettings();
            _zlibCompression = new ZlibCompression();

            _logger.LogInfo("Waiting for kafka connection",LogId.WaitingFor);
            _kafkaConnection = new KafkaConnection(_kafkaSettings);
            _logger.LogInfo("Connected to kafka",LogId.ConnectionSuccesful);

            _logger.LogInfo("waiting for mongo connection", LogId.WaitingFor);
            _mongoConnection = new MongoConnection(configProvider.ProvideMongoSettings());
            _logger.LogInfo("Connected to mongo", LogId.ConnectionSuccesful);

            _logger.LogInfo("Succesfuly initated mongo consumer", LogId.Initated);
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
            _logger.LogInfo("Started mongo consumer", LogId.Initated);
            while (true)
            {
                try
                {
                    ConsumeResult<Ignore,string> consumerResult = consumer.Consume(cancellationToken);
                    if (consumerResult.Topic != Consts.TELEMETRY_TOPIC_NAME)
                    {
                        string packetData = consumerResult.Message.Value.Split(Consts.PACKET_KAFKA_SPLIT)[1];
                        string packetDateTime = consumerResult.Message.Value.Split(Consts.PACKET_KAFKA_SPLIT)[0];
                        DateTime packetParsedDate = DateTime.ParseExact(packetDateTime, Consts.PACKET_DATETIME_FORMAT, CultureInfo.InvariantCulture);

                        BaseBoxCollection baseBoxCollection = new BaseBoxCollection();
                        baseBoxCollection.CompressedData = _zlibCompression.CompressData(packetData);
                        baseBoxCollection.IcdType = consumerResult.Topic;
                        baseBoxCollection.RealTime = packetParsedDate;
                        _mongoConnection.AddDocument(baseBoxCollection,_mongoSettings.DocumentTTL);
                    }
                }
                catch(KafkaException e)
                {
                    _logger.LogFatal("Tried receive data from kafka - "+e.Message, LogId.FatalKafkaReceive);
                }
                catch(Exception e)
                {
                    _logger.LogFatal("Tried receive data from kafka -"+e.Message, LogId.FatalKafkaReceive);
                }
            }
        }
    }
}
