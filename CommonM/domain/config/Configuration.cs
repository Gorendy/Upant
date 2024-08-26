using System.Collections.Generic;
using System.Xml.Serialization;

namespace CommonM.domain.config
{
    /// <summary>
    /// 程序配置文件信息
    /// </summary>
    [XmlRoot("UpantConfiguration")]
    public class Configuration
    {
        [XmlElement("SETTING")]
        public Setting setting { get; set; }
        [XmlElement("BACKUPSETTING")]
        public BackUpSetting backUpSetting { get; set; }
        [XmlElement("UPDATESETTING")]
        public UpdateSetting updateSetting { get; set; }
        [XmlArray("FTPINFOS")]
        public List<FTPInfo> ftpInfos { get; set; }
        [XmlArray("NODEMATCHERS")]
        public List<NodeMatcher> matchers { get; set; }
    }

    public class Setting
    {
        /// <summary>
        /// 配置文件路径(相对)，默认config开头
        /// </summary>
        [XmlElement("ConfigFileName")]
        public string configFile { get; set; }
        [XmlElement("ToBeUpdateNodeFile")]
        public string toBeUpdateNodeFile { get; set; }
        
        /// <summary>
        /// 忽略文件，支持文件夹格式
        /// </summary>
        [XmlElement("Ignores")]
        public Ignore ignores { get; set; }
    }

    public class BackUpSetting
    {
        /// <summary>
        /// 备份时忽略的文件
        /// </summary>
        [XmlElement("BackupIgnores")]
        public Ignore backupIgnores { get; set; }
    }

    public class UpdateSetting
    {
        /// <summary>
        /// 远程文件暂存路径
        /// </summary>
        [XmlElement("TmpPath")]
        public string tmpPath { get; set; }
        [XmlElement("UnzipPath")]
        public string unzipPath { get; set; }
    }
    /// <summary>
    /// 需要添加的配置节点 的同种类信息的xpath路径例如 name=add xpath = /add
    /// 解析待更新节点后根据节点名称获取xpath规则进行添加
    /// </summary>
    public class NodeMatcher
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlText]
        public string Xpath { get; set; }
    }
    /// <summary>
    /// FTP 远程服务器信息
    /// </summary>
    public class FTPInfo
    {
        /// <summary>
        /// ftp类型id,需唯一
        /// </summary>
        [XmlAttribute("ID")]
        public string ID { get; set; }

        [XmlElement]
        public string IP { get; set; }

        [XmlElement]
        public string Username { get; set; }

        [XmlElement]
        public string Password { get; set; }
    }
    /// <summary>
    /// 忽略文件格式
    /// </summary>
    public class Ignore
    {
        [XmlElement("Directory")]
        public List<string> directories { get; set; }
        [XmlElement("File")]
        public List<string> files { get; set; }
    }
}