using System;
using log4net;

namespace CommonM.logger
{

    /// <summary>
    /// 记录日志类
    /// </summary>
    public class Logger : LogBase
    {
        public delegate string MessageSend();
        public Logger(ILog log) : base(log) {
        }

        public Logger(string name) : base(name) {
        }

        public Logger(Type type) : base(type) {
        }

       

        public override void debug(RCode code, string message = null)
        {
            if (!log.IsDebugEnabled)
            {
                return;
            }

            log.Debug(msg(code, message));
        }
        public void debug(RCode code, MessageSend send)
        {
            if (!log.IsDebugEnabled)
            {
                return;
            }
            if (send != null)
                log.Debug(msg(code, send()));
        }

        public override void debug(RCode code, string message, Exception e)
        {
            if (!log.IsDebugEnabled)
            {
                return;
            }

            string s = msg(code, message);

            if (e == null)
            {
                log.Debug(s);
            }
            else
            {
                log.Debug(s, e);
            }
        }

        public override void error(RCode code, string message = null)
        {
            log.Error(errorMsg(code, message));
        }

        public override void error(RCode code, string message, Exception e)
        {
            
            string s = errorMsg(code, message);

            if (e == null)
            {
                log.Error(s);
            }
            else
            {
                log.Error(s, e);
            }
        }

        public override void info(RCode code, string message = null)
        {
            if (!log.IsInfoEnabled)
            {
                return;
            }

            log.Info(msg(code, message));
        }

        public override void info(RCode code, string message, Exception e)
        {
            string s = msg(code, message);

            if (e == null)
            {
                log.Info(s);
            }
            else
            {
                log.Info(s, e);
            }
        }

        public override void warn(RCode code, string message = null)
        {
            if (!log.IsWarnEnabled)
            {
                return;
            }

            log.Warn(warnMsg(code, message));
        }

        public override void warn(RCode code, string message, Exception e)
        {
            string s = warnMsg(code, message);

            if (e == null)
            {
                log.Warn(s);
            }
            else
            {
                log.Warn(s, e);
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

        public abstract void debug(RCode code, string message = null);
        public abstract void debug(RCode code, string message, Exception e);
        public abstract void error(RCode code, string message = null);
        public abstract void error(RCode code, string message, Exception e);
        public abstract void info(RCode code, string message = null);
        public abstract void info(RCode code, string message, Exception e);
        public abstract void warn(RCode code, string message = null);
        public abstract void warn(RCode code, string message, Exception e);

        protected string msg(RCode code, string message = null)
        {
            var messageBlock = Result.detail(code);
            string s = null;
            if (string.IsNullOrEmpty(message))
            {
                s = format(messageBlock);
            }
            else
            {
                s = format(messageBlock, message);
            }
            return s;
        }
        protected string warnMsg(RCode code, string message = null)
        {
            var messageBlock = Result.detail(code);
            string s = null;
            if (string.IsNullOrEmpty(message))
            {
                s = format(messageBlock);
            }
            else
            {
                s = format(messageBlock, message);
            }
            return s;
        }
        protected string errorMsg(RCode code, string message = null)
        {
            var messageBlock = Result.detail(code);
            string s = null;
            if (string.IsNullOrEmpty(message))
            {
                s = format(messageBlock);
            } else
            {
                s = format(messageBlock, message);
            }
            return s;
        }
        private string format(Result.MessageBlock message)
        {
            return $"ErCode:#{message.code},msg:{message.message}";
        }

        private string format(Result.MessageBlock message, string msg)
        {
            return $"ErCode:#{message.code},msg:{message.message},tips:{msg}";
        }
    }
}