namespace MongoConsumerLibary.MongoConnection
{
    public class MongoSettings
    {
        public string DataBaseName { get; set; }
        public string ConnectionUrl { get; set; }
        public int DocumentTTL { get; set; }
    }
}
