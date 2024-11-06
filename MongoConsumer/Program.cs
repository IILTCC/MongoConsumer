namespace MongoConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            Startup startUp = new Startup();
            startUp.StartMongoConsumer();
        }
    }
}
