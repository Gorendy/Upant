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
        private static readonly string defaultConfigPath = "conf";
        /// <summary>
        /// 查找程序config文件
        /// 优先级 conf/.config > ./.config
        /// </summary>
        /// <returns></returns>
        public static string findConfigFile(string path, string name) {
            logger.debug(RCode.CONF_INFO_FIND, () => $"find {name} in {path}");
            logger.info(RCode.CONF_INFO_FIND);
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(name))
            {
                logger.warn(RCode.CONF_WARN, $"params is null. path= '{path}', name='{name}'");
                return null;
            }

            if (!Directory.Exists(path))
            {
                logger.warn(RCode.FILE_NOT_EXIST, $"{path} is not directory");
                return null;
            }

            string result = null;
            if (!Directory.Exists(Path.Combine(path, defaultConfigPath)))
            {
                logger.debug(RCode.FILE_NOT_EXIST, () => $"{name} not found in {Path.Combine(path, defaultConfigPath)}");
            }
            else
            {
                result = FileUtil.findFile(Path.Combine(path, defaultConfigPath), name);
            }

            if (!string.IsNullOrEmpty(result))
            {
                logger.info(RCode.CONF_OK_FIND);
                return result;
            }

            result = FileUtil.findFile(path, name);
            if (string.IsNullOrEmpty(result))
            {
                logger.error(RCode.CONF_ERROR_FIND, $"{name} not found in {path} and ./{defaultConfigPath}");
            }
            else
            {
                logger.info(RCode.CONF_OK_FIND);
            }
            return result;
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
                using (var s = new FileStream(absoluteFilePath, FileMode.Create)) {
                    using (var writer = new StreamWriter(s, Encoding.UTF8)) {
                        serializer.Serialize(writer, obj, ns);// 序列化
                    }
                }
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_SERIALIZATION, $"'{obj.GetType().Name}' error", e);
                return;
            }
            logger.info(RCode.CONF_OK_SERIALIZATION);
        }

        public static Object deserialization(Type type, string absoluteFilePath) {
            logger.debug(RCode.CONF_INFO_DESERIALIZATION, $"'{type.Name}' will be deserialization");
            logger.info(RCode.CONF_INFO_DESERIALIZATION);
            if (type == null || string.IsNullOrEmpty(absoluteFilePath)) {
                logger.warn(RCode.CONF_WARN, $"obj:'{type.Name}',path:'{absoluteFilePath}' params is null");
                return null;
            }

            Object obj;
            try {
                XmlSerializer serializer = new XmlSerializer(type);
                FileStream fs = File.Open(absoluteFilePath, FileMode.Open);
                using (var reader = new StreamReader(fs, Encoding.UTF8)) {
                    obj = serializer.Deserialize(reader);
                }
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_DESERIALIZATION, $"'{type.Name}' deserialization unsuccessfully", e);
                return null;
            }
            logger.info(RCode.CONF_OK_DESERIALIZATION);
            logger.debug(RCode.CONF_OK_DESERIALIZATION, () => obj.ToString());
            return obj;
        }
    } 
}