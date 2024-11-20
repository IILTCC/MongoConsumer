using MongoConsumerLibary.MongoConnection.Collections;
using MongoConsumerLibary.MongoConnection.Enums;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoConsumerLibary.MongoConnection
{
    public class MongoConnection
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly Dictionary<CollectionType, IMongoCollection<BaseBoxCollection>> _collections;
        private readonly MongoSettings _mongoSettings;
        public MongoConnection(MongoSettings mongoSettings)
        {
            _mongoSettings = mongoSettings;
            _mongoClient = new MongoClient(_mongoSettings.ConnectionUrl);
            _database = _mongoClient.GetDatabase(_mongoSettings.DataBaseName);
            _collections = new Dictionary<CollectionType, IMongoCollection<BaseBoxCollection>>();
            foreach(CollectionType collectionType in Enum.GetValues(typeof(CollectionType)))
            {
                _collections.Add(collectionType,_database.GetCollection<BaseBoxCollection>(collectionType.ToString()));
            }
        }
        public void AddDocument(CollectionType collectionType, BaseBoxCollection baseBoxCollection)
        {
            _collections[collectionType].InsertOne(baseBoxCollection);
        }
    }
}
