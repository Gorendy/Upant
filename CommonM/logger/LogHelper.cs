using System;
using System.Collections.Generic;

namespace CommonM.logger
{
    public enum ResultCode
    {
        none,
    }

    public static class Result
    {
        
        private static readonly Dictionary<ResultCode, MessageBlock> messageList =
            new Dictionary<ResultCode, MessageBlock>()
            {
                { ResultCode.none, new MessageBlock(000_000, "未知错误！！")},
            };

        public static MessageBlock detail(ResultCode code) {
            if (!messageList.ContainsKey(code)) {
                return new MessageBlock(000_000, "未知类型错误");
            }

            return messageList[code];
        }
        public class MessageBlock
        {
            public readonly int code;
            public readonly string message;

            public MessageBlock(int code, string message) {
                this.code = code;
                this.message = message;
            }
        }
    }
    public interface ILogger
    {
        void debug(string message);
        void debug(string message, Exception exception);
        void info(string message);
        void info(string message, Exception exception);
        void warn(string message);
        void warn(string message, Exception exception);
        void error(string message);
        void fatal(string message);
        void error(string message, Exception exception);
        void fatal(string message, Exception exception);
    }
}

