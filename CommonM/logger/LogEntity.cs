using System;
using log4net;

namespace CommonM.logger
{

    public class Logger : LogBase
    {
        
        public Logger(ILog log) : base(log) {
        }

        

        public Logger(string name) : base(name) {
        }

        public Logger(Type type) : base(type) {
        }

        public override void debug(string message) {
            debug(message, null);
        }

        public override void debug(string message, Exception exception) {
            if (!log.IsDebugEnabled) {
                return;
            }

            if (string.IsNullOrEmpty(message)) {
                return;
            }

            if (exception == null) {
                log.Debug(message);
            }
            else {
                log.Debug(message, exception);
            }
        }

        public override void info(string message) {
            info(message, null);
        }

        public override void info(string message, Exception exception) {
            if (!log.IsInfoEnabled) {
                return;
            }

            if (string.IsNullOrEmpty(message)) {
                return;
            }

            if (exception == null) {
                log.Info(message);
            }
            else {
                log.Info(message, exception);
            }
        }

        public override void warn(string message) {
            warn(message, null);
        }

        public override void warn(string message, Exception exception) {
            if (!log.IsWarnEnabled) {
                return;
            }

            if (string.IsNullOrEmpty(message)) {
                return;
            }

            if (exception == null) {
                log.Warn(message);
            }
            else {
                log.Warn(message, exception);
            }
        }

        public override void error(string message) {
            error(message, null);
        }

        public override void fatal(string message) {
            fatal(message, null);
        }

        public override void error(string message, Exception exception) {
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            if (exception == null) {
                log.Error(message);
            }
            else {
                log.Error(message, exception);
            }
        }

        public override void fatal(string message, Exception exception) {
            if (string.IsNullOrEmpty(message)) {
                return;
            }

            if (exception == null) {
                log.Fatal(message);
            }
            else {
                log.Fatal(message, exception);
            }
        }
    }
    public abstract class LogBase: ILogger
    {
        protected readonly ILog log;

        protected LogBase(ILog log) {
            this.log = log;
        }

        protected LogBase(string name) {
            this.log = LogManager.GetLogger(name);
        }

        protected LogBase(Type type) {
            log = LogManager.GetLogger(type);
        }
        
        public abstract void debug(string message);
        public abstract void debug(string message, Exception exception);
        public abstract void info(string message);
        public abstract void info(string message, Exception exception);
        public abstract void warn(string message);
        public abstract void warn(string message, Exception exception);
        public abstract void error(string message);
        public abstract void fatal(string message);
        public abstract void error(string message, Exception exception);
        public abstract void fatal(string message, Exception exception);
    }
}