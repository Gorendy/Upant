using System;
using System.Collections.Generic;
using System.Xml;
using CommonM.logger;

namespace CommonM.util
{
    public class XmlUtil
    {
        private static readonly Logger logger = (Logger)LogFactory.getLogger(typeof(XmlUtil));
        /// <summary>
        /// 复制节点时，需要修改节点内容
        /// </summary>
        public delegate string ModifyContent(XmlNode node);

        /// <summary>
        /// 修改节点属性
        /// </summary>
        public delegate string ModifyAttribute(XmlAttribute attribute);

        #region 检索节点

        public static XmlNodeList selectNodeByName(XmlDocument doc, string nodeName) {
            
            return doc.SelectNodes($"//*[local-name()='{nodeName}']");
        }

        public static XmlNode selectSingleNodeByName(XmlDocument doc, string nodeName) {
            var list = doc.SelectNodes($"//*[local-name()='{nodeName}']");
            if (list == null)
            {
                return null;
            }
            return list.Item(0);
        }

        public static XmlNode selectSingleNodeByPattern(XmlDocument doc, string pattern) {
            return doc.SelectSingleNode(pattern);
        }

        public static XmlNodeList selectNodeByPattern(XmlDocument doc, string pattern) {
            return doc.SelectNodes(pattern);
        }

        public static XmlNodeList retrievesPattern(XmlNode node, string pattern) {
            return node.SelectNodes(pattern);
        }

        #endregion

        #region 复制

        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode) {
            copyNode(targetDoc, targetNode, sourceNode, sourceNode.Name, true, null,null);
        }
        
        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode, string newNode) {
            copyNode(targetDoc, targetNode, sourceNode, newNode?? sourceNode.Name, true, null,null);
        }
        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode, string newNode, ModifyContent contentCondition) {
            copyNode(targetDoc, targetNode, sourceNode, newNode?? sourceNode.Name, true, contentCondition,null);
        }
        /// <summary>
        /// 将节点复制到另一个文件中的某一节点上
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="targetNode"></param>
        /// <param name="sourceNode"></param>
        /// <param name="newName"></param>
        /// <param name="attributeCopy"></param>
        /// <param name="contentCondition">修改节点内容条件</param>
        /// <param name="attributeCondition">修改节点属性条件</param>
        /// <exception cref="Exception"></exception>
        public static void copyNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode, string newName,
            bool attributeCopy, ModifyContent contentCondition, ModifyAttribute attributeCondition) {
            if (targetDoc == null || targetNode == null || sourceNode == null) {
                logger.warn(RCode.CONF_WARN_XMLNODE, $"params is not found, xmlDoc:'{targetDoc}', target:'{targetNode}', source:'{sourceNode}'");
                return;
            }

            if (sourceNode.NodeType != XmlNodeType.Element) {
                return;
            }
            logger.info(RCode.CONF_INFO_ADD_XMLNODE);
            // 添加子节点
            XmlElement newNode = targetDoc.CreateElement(newName);

            // 如果父节点存在参数，判断是否复制参数
            if (sourceNode.Attributes != null && attributeCopy) {
                if (sourceNode.Attributes.Count != 0) {
                    if (attributeCondition == null) {// 非条件复制
                        // 添加子节点属性
                        foreach (XmlAttribute att in sourceNode.Attributes) {
                            newNode.SetAttribute(att.Name, att.Value);
                        }
                    }
                    else { // 条件复制
                        // 添加子节点属性
                        foreach (XmlAttribute att in sourceNode.Attributes) {
                            string value = attributeCondition(att);
                            newNode.SetAttribute(att.Name, string.IsNullOrEmpty(value)? att.Value : value);
                            
                        }
                    }
                }
                logger.debug(RCode.CONF_INFO_ADD_XMLNODE, () => $"{newNode.Name} node add attributes successfully");
            }
            // 存在子节点，对子节点复制
            if (sourceNode.HasChildNodes) {
                if (contentCondition != null) {
                    copyChildNode(targetDoc, newNode, sourceNode, contentCondition);
                }
                else {
                    copyChildNode(targetDoc, newNode, sourceNode);
                }
            }
            else {
                string si = null;
                if (contentCondition != null) {
                    si = contentCondition(newNode);
                }

                if (string.IsNullOrEmpty(si)) {
                    if (!string.IsNullOrEmpty(sourceNode.InnerText)) {
                        newNode.InnerText = sourceNode.InnerText;
                    }
                }
                else {
                    newNode.InnerText = si;
                }
            }

            targetNode.AppendChild(newNode);
            logger.info(RCode.CONF_OK_ADD_XMLNODE);
        }

        /// <summary>
        /// 复制子节点
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="targetNode"></param>
        /// <param name="sourceNode"></param>
        private static void copyChildNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode) {
            foreach (XmlNode child in sourceNode.ChildNodes) {
                if (child.NodeType == XmlNodeType.Comment) {
                    continue;
                }
                XmlNode newChild = targetDoc.ImportNode(child, true);
                
                targetNode.AppendChild(newChild);
                
            }
        }

        /// <summary>
        /// 复制子节点，会将源节点中的参数同时复制
        /// 只能修改节点内容，暂时不能修改节点属性
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="targetNode"></param>
        /// <param name="sourceNode"></param>
        /// <param name="contentCondition"></param>
        private static void copyChildNode(XmlDocument targetDoc, XmlNode targetNode, XmlNode sourceNode,
            ModifyContent contentCondition) {
            if (contentCondition == null) {
                return;
            }
            string s = null;
            foreach (XmlNode child in sourceNode.ChildNodes) {
                if (child.NodeType == XmlNodeType.Comment) {
                    continue;
                }
                // 节点不含有子节点
                var newChild = targetDoc.ImportNode(child, false);
                targetNode.AppendChild(newChild);
                if (hasChileNode(child)) { // 节点含有子节点
                    copyChildNode(targetDoc, newChild, child, contentCondition);
                    continue;
                }
                
                s = contentCondition(child);
                newChild.InnerText = !string.IsNullOrEmpty(s) ? s : child.InnerText;
            }
        }

        #endregion

        #region 修改节点内容，包括节点内容

        /// <summary>
        /// 更新单个节点属性
        /// </summary>
        /// <param name="targetDoc"></param>
        /// <param name="updateNode"></param>
        /// <param name="targetNode"></param>
        public static void updateNodeSingleAttribute(XmlDocument targetDoc, XmlNode updateNode, XmlNode targetNode) {
            if (targetNode == null || targetDoc == null || updateNode == null) {
                logger.warn(RCode.CONF_WARN_XMLNODE, $"params is not found, xmlDoc:'{targetDoc}', xmlNode:'{updateNode}'");
                return;
            }
            logger.info(RCode.CONF_INFO_UPT_XMLNODE);
            if (targetNode.Attributes.Count == 0) {
                return;
            }

            updateNode.Attributes.RemoveAll();
            foreach (XmlAttribute attr in targetNode.Attributes) {
                ((XmlElement)updateNode).SetAttribute(attr.Name, attr.Value);
            }
        }
        public static void updateNode(XmlDocument targetDoc, XmlNode updateNode, ModifyContent contentCondition) {
            if (contentCondition == null || targetDoc == null || updateNode == null) {
                logger.warn(RCode.CONF_WARN_XMLNODE, $"params is not found, xmlDoc:'{targetDoc}', xmlNode:'{updateNode}'");
                return;
            }
            logger.info(RCode.CONF_INFO_UPT_XMLNODE);
            if (!updateNode.HasChildNodes) {// 节点不包括子节点
                string s = contentCondition(updateNode);
                if (!string.IsNullOrEmpty(s))
                    updateNode.InnerText = s;
                logger.info(RCode.CONF_OK_UPT_XMLNODE);
                return;
            }
            // 对所有子节点进行内容修改
            Queue<XmlNode> queue = new Queue<XmlNode>();
            queue.Enqueue(updateNode);
            while (queue.Count != 0)
            {
                var tmp = queue.Dequeue();
                if (hasChileNode(tmp)) {
                    foreach (XmlNode child in tmp.ChildNodes) {
                        if (child.NodeType != XmlNodeType.Element) {
                            continue;
                        }
                        queue.Enqueue(child);
                    }
                }
                else {
                    string s = contentCondition(tmp);
                    if (!string.IsNullOrEmpty(s)) {
                        tmp.InnerText = s;
                    }
                }
            }
            logger.info(RCode.CONF_OK_UPT_XMLNODE);
        }

        #endregion
        /// <summary>
        /// 判断节点是否含有子标签节点，不包括type=test的类型
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static bool hasChileNode(XmlNode node) {
            if (node.NodeType != XmlNodeType.Element) {
                return false;
            }

            bool result = node.HasChildNodes;
            if (result) {
                if (node.ChildNodes.Count == 1 && node.FirstChild.NodeType == XmlNodeType.Text) {
                    result = false;
                }
            }
            return result;
        }
        /// <summary>
        /// 删除节点, 单个，多层
        /// 必须要有父节点
        /// </summary>
        /// <param name="node"></param>
        public static void delNode(XmlNode node) {
            if (node == null) {
                logger.warn(RCode.CONF_WARN_XMLNODE, $"node is not found");
                return;
            }
            logger.info(RCode.CONF_INFO_DEL_XMLNODE);
            var parent = node.ParentNode;
            if (parent != null) {
                parent.RemoveChild(node);
            }
            else {// 如果删除根节点
                logger.warn(RCode.CONF_WARN_XMLNODE, "the node deleted is root node");
                node.RemoveAll();
            }
            logger.info(RCode.CONF_OK_ADD_XMLNODE);
        }
    }
}