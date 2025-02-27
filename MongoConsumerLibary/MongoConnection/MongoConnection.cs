using Archive.Logs;
using MongoConsumerLibary.MongoConnection.Collections;
using MongoConsumerLibary.MongoConnection.Collections.PropetyClass;
using MongoConsumerLibary.MongoConnection.Enums;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoConsumerLibary.MongoConnection
{
    public class MongoConnection
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly CollectionEntity<BaseBoxCollection> _baseBoxCollection;
        private readonly CollectionEntity<StatisticCollection> _statisticCollection;

        private readonly MongoSettings _mongoSettings;
        public MongoConnection(MongoSettings mongoSettings)
        {
            _mongoSettings = mongoSettings;
            _mongoClient = new MongoClient(_mongoSettings.ConnectionUrl);
            _database = _mongoClient.GetDatabase(_mongoSettings.DataBaseName);
            WaitForMongoConnection();
            _baseBoxCollection = new CollectionEntity<BaseBoxCollection>(_database, nameof(BaseBoxCollection));
            _statisticCollection = new CollectionEntity<StatisticCollection>(_database, nameof(StatisticCollection));
        }
        public MongoConnection(MongoSettings mongoSettings,ArchiveLogger archiveLogger)
        {
            _mongoSettings = mongoSettings;
            _mongoClient = new MongoClient(_mongoSettings.ConnectionUrl);
            _database = _mongoClient.GetDatabase(_mongoSettings.DataBaseName);
            archiveLogger.LogInfo("Trying to connect to mongo",LogId.WaitingFor);
            WaitForMongoConnection();
            archiveLogger.LogInfo("Connection succesful",LogId.ConnectionSuccesful);
            _baseBoxCollection = new CollectionEntity<BaseBoxCollection>(_database,nameof(BaseBoxCollection), archiveLogger);
            _statisticCollection = new CollectionEntity<StatisticCollection>(_database, nameof(StatisticCollection));
        }
        public void WaitForMongoConnection()
        {
            bool isConnected = false;
            while (!isConnected)
                isConnected = _database.RunCommandAsync((Command<BsonDocument>)"{ping:1}").Wait(1000);
            
        }
        public void AddDocument(BaseBoxCollection document, int expireAfter)
        {
            _baseBoxCollection.AddDocument(document, expireAfter);
        }        
        public void AddDocument(StatisticCollection document, int expireAfter)
        {
            _statisticCollection.AddDocument(document, expireAfter);
        }
        public async Task<long> GetDocumentCount(DateTime startDate, DateTime endDate, IcdType icdType)
        {
            return await _baseBoxCollection.GetDocumentCount(startDate, endDate, icdType);
        }
        public async Task<long> GetStatisticsCount(DateTime startDate, DateTime endDate)
        {
            return await _statisticCollection.GetStatisticsCount(startDate, endDate);
        }
        public async Task<List<BaseBoxCollection>> GetDocument(int limit, int skip, DateTime startDate, DateTime endDate)
        {
            return await _baseBoxCollection.GetDocument( limit,skip,startDate,endDate);
        }
        public async Task<List<StatisticCollection>> GetStatisticDocument(int limit, int skip, DateTime startDate, DateTime endDate)
        {
            return await _statisticCollection.GetDocument(limit, skip, startDate, endDate);
        }
        public async Task<List<BaseBoxCollection>> GetDocument(IcdType collectionType, int limit,DateTime startDate)
        {
            return await _baseBoxCollection.GetDocument(collectionType, limit,startDate);
        }
        public async Task<List<BaseBoxCollection>> GetDocument(IcdType collectionType, int limit, int skip, DateTime startDate,DateTime endDate)
        {
            return await _baseBoxCollection.GetDocument(collectionType, limit,skip,startDate,endDate);
        }

    }
}
