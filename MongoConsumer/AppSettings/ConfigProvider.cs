using Microsoft.Extensions.Configuration;
using MongoConsumerLibary;
using MongoConsumerLibary.KafkaConsumer;
using MongoConsumerLibary.MongoConnection;
using System.IO;

namespace MongoConsumer.AppSettings
{
    class ConfigProvider
    {
        private static ConfigProvider _instance;
        private static IConfigurationRoot _configFile;
        private KafkaSettings _kafkaSettings;
        private MongoSettings _mongoSettings;
        public static ConfigProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConfigProvider();
                return _instance;
            }
        }
        public ConfigProvider()
        {
            _configFile = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(Consts.APPSETTINGS_PATH, optional: false, reloadOnChange: true)
            .Build();
            _kafkaSettings = _configFile.GetRequiredSection(nameof(KafkaSettings)).Get<KafkaSettings>();
            _mongoSettings = _configFile.GetRequiredSection(nameof(MongoSettings)).Get<MongoSettings>();
        }
        public KafkaSettings ProvideKafkaSettings()
        {
            return _kafkaSettings;                 
        }
        public MongoSettings ProvideMongoSettings()
        {
            return _mongoSettings;
        }
    }
}
