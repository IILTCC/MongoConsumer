using Microsoft.Extensions.Configuration;
using MongoConsumer.AppSettings;
using System;
using System.IO;
using MongoConsumerLibary.KafkaConsumer;
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
