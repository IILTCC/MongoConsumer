namespace MongoConsumerLibary.KafkaConsumer
{
    public class KafkaSettings
    {
        public string KafkaUrl { get; set; }
        public string KafkaConsumerGroup { get; set; }
        public string[] KafkaTopics { get; set; }
    }
}
