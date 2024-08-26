using System;
using System.Collections.Generic;
using System.Xml;
using CommonM.domain.constant;
using CommonM.domain.vo;
using CommonM.util;

namespace CommonM.domain.xml
{
    /// <summary>
    /// 收集待更新节点的配置文件类
    /// </summary>
    [Obsolete]
    public class UpdateXml:AbstractXml
    {
        private List<UpdateNodeVO> nodeVos;
        private bool isEmpty = false;
        public UpdateXml(string filePath) : base(filePath) {
            getUpdateNode();
        }

        public List<UpdateNodeVO> getUpdateNode() {
            if (nodeVos != null || isEmpty) {
                return nodeVos;
            }

            nodeVos = new List<UpdateNodeVO>();
            // 根据xpath规则获取父节点信息 默认规则 /UPDATENODE
            var parent = XmlUtil.selectSingleNodeByPattern(doc, Constant.updateFileXpath);
            if (parent == null || !parent.HasChildNodes) {
                isEmpty = true;
                nodeVos = null;
                return nodeVos;
            }

            var children = parent.ChildNodes;
            UpdateNodeVO tmp = new UpdateNodeVO();
            foreach (XmlNode node in children) {
                if (node.NodeType == XmlNodeType.Comment) {
                    tmp.getComments().Add(node);
                }
                else {
                    tmp.Node = node;
                    nodeVos.Add(tmp);
                    tmp = new UpdateNodeVO();
                }
            }
            return nodeVos;
        }
        public override void copyNode(string xpath, XmlNode sourceNode) {
        }

        public override void deleteNode(string matcher) {
        }

        public override void updateNode(string xpath, XmlNode source) {
        }
    }
}