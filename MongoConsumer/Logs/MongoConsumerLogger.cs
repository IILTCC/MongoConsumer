using NLog;

namespace MongoConsumer.Logs
{
    class MongoConsumerLogger
    {
        private Logger _logger;
        private static MongoConsumerLogger _instance; 
        public static MongoConsumerLogger Instance
        {
            get 
            {
                if (_instance == null)
                    _instance = new MongoConsumerLogger();
                return _instance;
            }
        }
        public MongoConsumerLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }
        public void LogInfo(string log)
        {
            _logger.Info(log);
        }

        public void LogWarn(string log)
        {
            _logger.Warn(log);
        }

        public void LogFatal(string log)
        {
            _logger.Fatal(log);
        }

        public void LogError(string log)
        {
            _logger.Error(log);
        }
    }
}
