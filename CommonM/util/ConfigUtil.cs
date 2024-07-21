using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using CommonM.logger;

namespace CommonM.util
{
    public class ConfigUtil
    {
        private  static readonly Logger logger = (Logger) LogFactory.getLogger(typeof(ConfigUtil));
        public static string findConfigFile() {
            return null;
        }

        /// <summary>
        /// 序列化对象到文件中
        /// </summary>
        /// <param name="type"></param>
        /// <param name="absoluteFilePath">目标文件位置</param>
        public static void serialization(Object obj, string absoluteFilePath) {
            logger.debug(RCode.CONF_INFO_SERIALIZATION, () => $"'{obj.GetType().Name}' will be serialization");
            logger.info(RCode.CONF_INFO_SERIALIZATION);
            if (obj == null || string.IsNullOrEmpty(absoluteFilePath)) {
                logger.warn(RCode.CONF_WARN, $"obj:'{obj}',path:'{absoluteFilePath}' params is null");
                return;
            }
            // 去除命名空间
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            try {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                if (!File.Exists(absoluteFilePath)) {
                    using (var s = new FileStream(absoluteFilePath, FileMode.OpenOrCreate)) {
                        using (var writer = new StreamWriter(s, Encoding.UTF8)) {
                            serializer.Serialize(writer, obj, ns);// 序列化
                        }
                    }
                }
                else {
                    using (var s = new FileStream(absoluteFilePath, FileMode.Truncate)) {
                        using (var writer = new StreamWriter(s, Encoding.UTF8)) {
                            serializer.Serialize(writer, obj, ns);// 序列化
                        }
                    }
                }
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_SERIALIZATION, $"'{obj.GetType().Name}' error", e);
                return;
            }
            logger.info(RCode.CONF_OK_SERIALIZATION);
        }

        public static Object deserialization(Object obj, string absoluteFilePath) {
            logger.debug(RCode.CONF_INFO_DESERIALIZATION, $"'{obj.GetType().Name}' will be deserialization");
            logger.info(RCode.CONF_INFO_DESERIALIZATION);
            if (obj == null || string.IsNullOrEmpty(absoluteFilePath)) {
                logger.warn(RCode.CONF_WARN, $"obj:'{obj}',path:'{absoluteFilePath}' params is null");
                return null;
            }

            try {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                FileStream fs = File.Open(absoluteFilePath, FileMode.Open);
                using (var reader = new StreamReader(fs, Encoding.UTF8)) {
                    obj = serializer.Deserialize(reader);
                }
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_DESERIALIZATION, $"'{obj.GetType().Name}' deserialization unsuccessfully", e);
                return null;
            }
            logger.info(RCode.CONF_OK_DESERIALIZATION);
            logger.debug(RCode.CONF_OK_DESERIALIZATION, () => obj.ToString());
            return obj;
        }
    } 
}