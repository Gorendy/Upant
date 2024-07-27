using System.Collections.Generic;
using System.Xml.Serialization;

namespace CommonM.domain.config
{
    /// <summary>
    /// 更新程序中的配置文件类
    /// </summary>
    [XmlRoot("SubConfiguration")]
    public class SubConfigure
    {
        [XmlElement("SubSetting")]
        public SubSetting setting { get; set; }
        
        [XmlElement("Nodes")]
        public UpdateInfo info { get; set; }
    }

    /// <summary>
    /// 该更新程序一些基本设置
    /// </summary>
    public class SubSetting
    {
        public string fileName;
        public string exeName;
    }

    /// <summary>
    /// 需要更新配置文件节点信息
    /// </summary>
    public class UpdateInfo
    {
        [XmlElement("Node")]
        public List<Node> nodes;
    }

    public class Node
    {
        [XmlText]
        public string xpath;
        [XmlAttribute]
        public string operate;

        public Node(string xpath, string operate)
        {
            this.xpath = xpath;
            this.operate = operate;
        }

        public Node()
        {
        }
    }
}