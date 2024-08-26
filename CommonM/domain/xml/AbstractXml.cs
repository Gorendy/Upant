using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommonM.logger;
using CommonM.util;

namespace CommonM.domain.xml
{
    public abstract class AbstractXml
    {
        protected readonly XmlDocument doc;
        protected readonly string file;
        protected readonly string fileName;
        protected readonly Logger logger;

        protected int copyNum = 0;
        protected int updateNum = 0;

        protected AbstractXml(string filePath) {
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

        #region abstract

        public abstract void copyNode(string xpath, XmlNode sourceNode);
        public abstract void deleteNode(string matcher);
        public abstract void updateNode(string xpath, XmlNode source);

        #endregion

        public string getFileName() {
            return fileName;
        }
        public void copyNodeList(string xpath, List<XmlNode> nodes) {
            if (string.IsNullOrEmpty(xpath)) {
                logger.warn(RCode.CONF_WARN_XMLNODE, "xpath is null");
                return;
            }

            int num = 0;
            try {
                num = XmlUtil.copyNodeList(doc, findNodeByPattern(xpath), nodes);
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR_ADD_XMLNODE,$"{fileName} copy node error", e);
            }

            if (num != nodes.Count) {
                logger.error(RCode.CONF_ERROR_ADD_XMLNODE,$"{fileName} copy node num is fail");
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

        public int getAddNum() {
            return copyNum;
        }

        public int getUpdateNum() {
            return updateNum;
        }
        public void save()
        {
            try {
                doc.Save(file);
                copyNum = 0;
                updateNum = 0;
            }
            catch (Exception e) {
                logger.error(RCode.CONF_ERROR, $"{fileName} save error", e);
                throw;
            }
        }
    }
}