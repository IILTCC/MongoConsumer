using MongoConsumerLibary.MongoConnection.Collections;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoConsumerLibary.MongoConnection
{
    public class MongoConnection
    {
        private readonly MongoClient _mongoClient;
        private readonly IMongoDatabase _database;
        private readonly CollectionEntity<BaseBoxCollection> _baseBoxCollection;

        private readonly MongoSettings _mongoSettings;
        public MongoConnection(MongoSettings mongoSettings)
        {
            _mongoSettings = mongoSettings;
            _mongoClient = new MongoClient(_mongoSettings.ConnectionUrl);
            _database = _mongoClient.GetDatabase(_mongoSettings.DataBaseName);
            WaitForMongoConnection();
            _baseBoxCollection = new CollectionEntity<BaseBoxCollection>(_database,nameof(BaseBoxCollection));
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
    }
}
