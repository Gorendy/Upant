using System;
using System.IO;
using System.Xml;
using CommonM.logger;
using CommonM.util;

namespace CommonM.domain
{
    public class XmlFile
    {
        private readonly XmlDocument doc;
        private readonly string file;
        private readonly string fileName;
        private readonly Logger logger;

        public delegate void ModifyFile();

        public XmlFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new Exception($"{filePath} not found");
            }
            file = filePath;
            fileName = Path.GetFileName(filePath);
            logger = (Logger)LogFactory.getLogger(fileName);
            try
            {
                doc = new XmlDocument();
                doc.Load(filePath);
            }
            catch (Exception e)
            {
                logger.error(RCode.CONF_ERROR, $"{fileName} can not load", e);
            }
        }

        public XmlNode findNodeByName(string name)
        {
            return XmlUtil.selectSingleNodeByName(doc, name);
        }

        public XmlNode findNodeByPattern(string xpath)
        {
            return XmlUtil.selectSingleNodeByPattern(doc, xpath);
        }

        public void copyNode(string xpath, XmlNode sourceNode)
        {
            if (sourceNode == null || sourceNode.NodeType == XmlNodeType.Comment)
            {
                logger.info(RCode.CONF_WARN_XMLNODE);
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

        public void deleteNode(string matcher)
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

        public void updateAndSave(ModifyFile modify)
        {
            modify();
            save();
        }

        public void save()
        {
            doc.Save(file);
        }
    }
}