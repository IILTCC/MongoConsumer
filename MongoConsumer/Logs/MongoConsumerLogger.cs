using MongoConsumerLibary;
using NLog;
using System.Reflection;

namespace MongoConsumer.Logs
{
    class MongoConsumerLogger
    {
        private Logger _logger;
        private static MongoConsumerLogger _instance;
        private readonly string _hostname;

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
            ScopeContext.PushProperty(Consts.LOGGER_VARIABLE_NAME, Consts.PROJECT_NAME);
            Assembly assembly = Assembly.GetExecutingAssembly();
            _hostname = assembly.GetName().Name;
        }
        public LogEventInfo ConstructLog(string log, LogId id)
        {
            LogEventInfo logEvent = new LogEventInfo(LogLevel.Info, _logger.Name, log)
            {
                Level = LogLevel.Info,
                Properties = { ["IPAddress"] = Consts.LOGGER_IP, ["ProjectName"] = _hostname, ["Id"] = (int)id }
            };
            return logEvent;
        }

        public void LogInfo(string log,LogId id)
        {
            _logger.Info(ConstructLog(log,id));
        }

        public void LogWarn(string log, LogId id)
        {
            _logger.Warn(ConstructLog(log,id));
        }

        public void LogFatal(string log, LogId id)
        {
            _logger.Fatal(ConstructLog(log,id));
        }
        
        public void LogError(string log, LogId id)
        {
            _logger.Error(ConstructLog(log,id));
        }
    }
}
