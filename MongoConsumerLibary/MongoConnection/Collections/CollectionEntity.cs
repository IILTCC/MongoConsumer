using MongoDB.Driver;
using System;

namespace MongoConsumerLibary.MongoConnection.Collections
{
    class CollectionEntity<CollectionType> where CollectionType : ExpirableCollection
    {
        private readonly IMongoCollection<CollectionType> _collection;
        public CollectionEntity(IMongoDatabase database,string collectionName)
        {
            _collection = database.GetCollection<CollectionType>(collectionName);
            AddIndex();
        }

        public void AddIndex()
        {
            CreateIndexOptions indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
            IndexKeysDefinition<CollectionType> indexKeys = Builders<CollectionType>.IndexKeys.Ascending(document => document.ExpirationTime);
            _collection.Indexes.CreateOne(new CreateIndexModel<CollectionType>(indexKeys, indexOptions));
        }

        public void AddDocument(CollectionType document, int expireAfter)
        {
            document.InsertTime = DateTime.Now;
            document.ExpirationTime = DateTime.Now.AddSeconds(expireAfter);
            _collection.InsertOne(document);
        }
    }
}
