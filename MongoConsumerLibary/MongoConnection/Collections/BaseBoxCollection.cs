using System;

namespace MongoConsumerLibary.MongoConnection.Collections
{
    public class BaseBoxCollection
    {
        public DateTime ExpirationTime { get; set; }
        public string CompressedData { get; set; }
    }
}
