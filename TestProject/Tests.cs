using System;
using CommonM.logger;
using CommonM.util;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class Tests
    {
        private Logger logger = (Logger)LogFactory.getLogger(typeof(Tests));

        public string backUpDirName => $"bak_{DateTime.Now:yyyyMMdd}";

        [PreTest]
        public void before() {
            Console.WriteLine("test pre");
            LogFactory.initLog();
        }

        [Test]
        public void backupTest() {
            logger.info(RCode.FILE_INFO_COPY, "文件即将备份");
            string path = @"C:\localfile\test\local";
            FileUtil.copyDirectory(path, path, backUpDirName);
            logger.info(RCode.FILE_OK_COPY);
        }
    }
}