using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace CommonM.domain.vo
{
    /// <summary>
    /// 待更新节点信息
    /// </summary>
    public class UpdateNodeVO
    {
        private List<XmlNode> comment = new List<XmlNode>();
        private List<XmlNode> all;
        private UpdateType type;
        private XmlNode node;
        public UpdateNodeVO() {
            type = UpdateType.ADD;
        }
        public UpdateNodeVO(UpdateType type) {
            this.type = type;
        }
        public List<XmlNode> getComments() {
            return comment;
        }
        

        public XmlNode Node {
            get => node;
            set => node = value;
        }

        public List<XmlNode> getAllXmlNodes() {
            if (all != null) {
                return all;
            }

            all = new List<XmlNode>(comment.Count + 1);
            foreach (XmlNode node in comment) {
                all.Add(node);
            }
            all.Add(node);
            return all;
        }

        public string getSearchMatcher() {
            StringBuilder si = new StringBuilder();
            switch (type) {
                case UpdateType.ADD:
                    if (node.Attributes != null) {
                        XmlNode item = node.Attributes.Item(0);
                        si.Append("/add[@").Append(item.Name).Append("='").Append(item.Value).Append("']");
                    }
                    else {
                        si.Append("/None");
                    }
                    break;
                default:
                    si.Append("/None");
                    break;
            }
            return si.ToString();
        }
        public bool hasComments() {
            return comment.Count != 0;
        }
    }
    /// <summary>
    /// 更新节点类型
    /// </summary>
    public enum UpdateType
    {
        ADD,
    }
}