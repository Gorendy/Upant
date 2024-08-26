using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommonM.domain.constant;
using CommonM.logger;
using CommonM.util;

namespace CommonM.domain.vo
{
    /// <summary>
    /// 待更新配置文件类
    /// 包含需要更新到目标程序的节点信息
    /// </summary>
    public class ToBeUpdateConf
    {
        private List<UpdateNodeVO> nodeVos;
        private string fileName;
        public ToBeUpdateConf(string filePath) {
            initNodeFromConfig(filePath);
        }

        private void initNodeFromConfig(string filePath) {
            if (string.IsNullOrEmpty(filePath) || !FileUtil.hasFile(filePath)) {
                throw new Exception($"{filePath} is not found");
            }

            fileName = Path.GetFileName(filePath);
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);
            nodeVos = new List<UpdateNodeVO>();
            // 根据xpath规则获取父节点信息 默认规则 /UPDATENODE
            var parent = XmlUtil.selectSingleNodeByPattern(doc, Constant.updateFileXpath);
            if (parent == null || !parent.HasChildNodes) {
                nodeVos = null;
                return;
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
        }

        public string getFileName() {
            return fileName;
        }
        public List<UpdateNodeVO> getUpdateNode() {
            return nodeVos;
        }
    }
}