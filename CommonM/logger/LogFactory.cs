using System;
using log4net;

namespace CommonM.logger
{
    public static class LogFactory
    {
        public static ILogger getLogger() {
            return new Logger("UpantService");
        }

        public static ILogger getLogger(Type type) {
            return new Logger(type);
        }
        
        
    }
}