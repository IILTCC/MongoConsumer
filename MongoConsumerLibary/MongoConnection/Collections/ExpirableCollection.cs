using System;

namespace MongoConsumerLibary.MongoConnection.Collections
{
    public class ExpirableCollection
    {
        public DateTime InsertTime { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
