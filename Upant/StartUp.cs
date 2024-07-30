using System;
using CommonM.domain.config;
using CommonM.logger;
using CommonM.util;
using Upant.context;

namespace Upant
{
    public class StartUp
    {
        private static ILogger logger =  LogFactory.getLogger(typeof(StartUp));
        private static readonly string path = AppDomain.CurrentDomain.BaseDirectory;
        private static readonly string configName = "conf.xml";
        public static void initConfigInfo() {
            try {
                string config =  ConfigUtil.findConfigFile(path, configName);
                DataContext.config = (Configure) ConfigUtil.deserialization(typeof(Configure), config);
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR, "程序主配置文件加载失败");
                throw;
            }
            logger.info(RCode.CONF_OK_DESERIALIZATION);
        }
        public static void initSubConfigInfo() {
            try {
                string config =  ConfigUtil.findConfigFile(path, configName);
                DataContext.subConfigure = (SubConfigure) ConfigUtil.deserialization(typeof(SubConfigure), config);
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR, "程序主配置文件加载失败");
                throw;
            }
            logger.info(RCode.CONF_OK_DESERIALIZATION);
        }
    }
}