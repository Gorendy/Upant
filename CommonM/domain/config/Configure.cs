using System.Collections.Generic;
using System.Xml.Serialization;

namespace CommonM.domain.config
{
    /// <summary>
    /// 程序序列化类，将配置文件进行序列化操作
    /// </summary>
    [XmlRoot("UpantConfiguration")]
    public class Configure
    {
        /// <summary>
        /// 本程序配置信息
        /// </summary>
        [XmlElement("Setting")]
        public Setting setting { get; set; }
        /// <summary>
        /// 次级配置信息，保存子配置文件信息（目标更新文件中配置信息）
        /// </summary>
        [XmlElement("SubConfiguration")]
        public SubConfig config { get; set; }

        public override string ToString() {
            return $"Configure:[setting='{setting}', subConfig='{config}']";
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Setting
    {
        [XmlElement("LocalPath")]
        public string localPath { get; set; }
        [XmlElement("RemotePath")]
        public string remotePath { get; set; }
        /// <summary>
        /// 配置文件名称，默认config开头
        /// </summary>
        [XmlElement("ConfigFileName")]
        public string configFileName { get; set; }
        /// <summary>
        /// 忽略文件，支持文件夹格式
        /// </summary>
        [XmlElement("Ignores")]
        public List<Ignore> ignores { get; set; }

        public override string ToString() {
            return $"Setting:[localPath='{localPath}', remotePath='{remotePath}', configFileName='{configFileName}', ignores={ignores}]";
        }
    }

    public class SubConfig
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlElement("UpdateConfigName")]
        public string updateConfigName { get; set; }
        /// <summary>
        /// 执行程序配置文件（待更新配置文件）
        /// </summary>
        [XmlElement("ExecuteConfigFile")]
        public string executeConfigFile { get; set; }
        [XmlElement("ExecuteProgress")]
        public string executeProgress { get; set; }

        public override string ToString() {
            return $"SubConfig: [updateConfigName='{updateConfigName}', executeConfigFile='{executeConfigFile}', executeProgress='{executeProgress}']";
        }
    }

    public class Ignore
    {
        [XmlElement("Directory")]
        public List<string> directories { get; set; }
        [XmlElement("files")]
        public List<string> files { get; set; }
        
    }
}