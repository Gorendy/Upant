using System;
using log4net;

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
        
        
    }
}