using Microsoft.Extensions.Configuration;
using MongoConsumer.AppSettings;
using System;
using System.IO;
using MongoConsumerLibary.KafkaConsumer;
namespace MongoConsumer
{
    class Program
    {
        private static IConfigurationRoot _configFile;
        static void Main(string[] args)
        {
            _configFile = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
          .AddJsonFile(AppSettingPaths.AppSettingName, optional: false, reloadOnChange: true)
          .Build();

            KafkaSettings kafkaSettings = _configFile.GetRequiredSection(AppSettingPaths.KafkaSettingsPath).Get<KafkaSettings>();
            Startup startUp = new Startup(kafkaSettings);
            startUp.StartMongoConsumer();
        }
    }
}
