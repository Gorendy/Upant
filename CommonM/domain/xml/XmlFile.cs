using System;
using System.IO;
using System.Xml;
using CommonM.logger;
using CommonM.util;

namespace CommonM.domain.xml
{
    public class XmlFile:AbstractXml
    {

        public XmlFile(string filePath):base(filePath)
        {
            
        }


        public override void copyNode(string xpath, XmlNode sourceNode)
        {
            if (sourceNode == null || sourceNode.NodeType == XmlNodeType.Comment)
            {
                logger.warn(RCode.CONF_WARN_XMLNODE);
                return;
            }

            try
            {
                copyNode(xpath, sourceNode, sourceNode.Name, true, null);
            }
            catch (Exception e)
            {
                logger.error(RCode.CONF_ERROR_ADD_XMLNODE, $"{fileName} is not add new node", e);
            }
           
        }

        public void copyNode(string xpath, XmlNode sourceNode, string name, bool copyAttribute,
            XmlUtil.ModifyContent contentCondition)
        {
            XmlUtil.copyNode(doc, findNodeByPattern(xpath), sourceNode, name, copyAttribute, contentCondition, null);
        }

        public void copyNode(string xpath, XmlNode sourceNode, XmlUtil.ModifyContent contentCondition)
        {
            XmlUtil.copyNode(doc, findNodeByPattern(xpath), sourceNode, sourceNode.LocalName, contentCondition);
        }

        public override void deleteNode(string matcher)
        {
            if (string.IsNullOrEmpty(matcher))
            {
                return;
            }

            XmlNode tmp;
            if (matcher.Contains("/"))
            {
                tmp = findNodeByPattern(matcher);
            }
            else
            {
                tmp = findNodeByName(matcher);
            }

            XmlUtil.delNode(tmp);
        }

        public void updateNode(string xpath, XmlUtil.ModifyContent contentCondition)
        {
            XmlUtil.updateNode(doc, findNodeByPattern(xpath), contentCondition);
        }

        public override void updateNode(string xpath, XmlNode source) {
            try {
                XmlUtil.updateNodeSingleAttribute(findNodeByPattern(xpath), source);
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_UPT_XMLNODE, $"{fileName} update error", e);
            }
            
        }

        
    }
}