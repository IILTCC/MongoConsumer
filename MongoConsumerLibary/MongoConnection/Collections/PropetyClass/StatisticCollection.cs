using System;
using System.Collections.Generic;

namespace MongoConsumerLibary.MongoConnection.Collections.PropetyClass
{
    public class StatisticCollection : ExpirableCollection
    {
        public StatisticCollection()
        {
            StatisticValues = new Dictionary<string, StatisticDictValue>();
        }
        public Dictionary<string, StatisticDictValue> StatisticValues { get; set; }
    }
}
