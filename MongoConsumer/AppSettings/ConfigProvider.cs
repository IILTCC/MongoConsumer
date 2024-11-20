using Microsoft.Extensions.Configuration;
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
            .AddJsonFile(AppSettingPaths.AppSettingName, optional: false, reloadOnChange: true)
            .Build();
            _kafkaSettings = _configFile.GetRequiredSection(AppSettingPaths.KafkaSettingsPath).Get<KafkaSettings>();
            _mongoSettings = _configFile.GetRequiredSection(AppSettingPaths.MongoSettingsPath).Get<MongoSettings>();
        }
        public KafkaSettings KafkaSettings()
        {
            return _kafkaSettings;                 
        }
        public MongoSettings MongoSettings()
        {
            return _mongoSettings;
        }
    }
}
