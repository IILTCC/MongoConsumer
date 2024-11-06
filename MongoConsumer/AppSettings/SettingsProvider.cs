using Microsoft.Extensions.Configuration;
using MongoConsumerLibary.KafkaConsumer;
using System.IO;

namespace MongoConsumer.AppSettings
{
    class SettingsProvider
    {
        private static SettingsProvider _instance;
        private static IConfigurationRoot _configFile;
        private KafkaSettings _kafkaSettings;
        public static SettingsProvider Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SettingsProvider();
                return _instance;
            }
        }
        public SettingsProvider()
        {
            _configFile = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile(AppSettingPaths.AppSettingName, optional: false, reloadOnChange: true)
            .Build();
            _kafkaSettings = _configFile.GetRequiredSection(AppSettingPaths.KafkaSettingsPath).Get<KafkaSettings>();
        }
        public KafkaSettings ProvideKafkaSettings()
        {
            return _kafkaSettings;                 
        }
    }
}
