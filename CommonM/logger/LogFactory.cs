using System;
using System.IO;
using log4net;
using log4net.Config;

namespace CommonM.logger
{
    public static class LogFactory
    {
        private static ILogger _logger;
        public static ILogger getLogger() {
            if (_logger == null) {
                lock (_logger) {
                    if (_logger == null) {
                        _logger = new Logger("UpantService");
                    }
                }
            }

            return _logger;
        }

        public static ILogger getLogger(Type type) {
            return new Logger(type);
        }
        public static ILogger getLogger(string name) {
            return new Logger(name);
        }
        public static void initLog() {
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "log.xml");
            FileInfo configFile = new FileInfo(configFilePath);
            XmlConfigurator.Configure(configFile);
        }
        
    }
}