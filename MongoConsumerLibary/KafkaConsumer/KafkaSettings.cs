using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoConsumerLibary.KafkaConsumer
{
    public class KafkaSettings
    {
        public string KafkaUrl { get; set; }
        public string KafkaConsumerGroup { get; set; }
    }
}
