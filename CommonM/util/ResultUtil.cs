using CommonM.logger;

namespace CommonM.util
{
    /// <summary>
    /// 提示信息工具类
    /// 将信息和编号进行合并展示
    /// </summary>
    public class ResultUtil
    {
        
        public static string msg(RCode code) {
            var messageBlock = Result.detail(code);
            return format(messageBlock);
        }

        public static string msg(RCode code, string message) {
            var messageBlock = Result.detail(code);
            return format(messageBlock, message);
        }
        public static string error(RCode code) {
            var messageBlock = Result.detail(code);
            return format(messageBlock);
        }

        public static string error(RCode code, string message) {
            var messageBlock = Result.detail(code);
            return format(messageBlock, message);
        }

        public static string ok(RCode code) {
            var messageBlock = Result.detail(code);
            return format(messageBlock);
        }
        public static string ok(RCode code, string message) {
            var messageBlock = Result.detail(code);
            return format(messageBlock, message);
        }
        private static string format(Result.MessageBlock message) {
            return $"ErCode:#{message.code},msg:{message.message}";
        }

        private static string format(Result.MessageBlock message, string msg) {
            return $"ErCode:#{message.code},msg:{message.message},tips:{msg}";
        }
    }
}