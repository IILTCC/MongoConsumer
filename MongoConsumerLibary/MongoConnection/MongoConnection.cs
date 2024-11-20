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
        public MongoConnection()
        {
            _mongoClient = new MongoClient("mongodb://localhost:27017");
            _database = _mongoClient.GetDatabase("telemetryDb");
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
