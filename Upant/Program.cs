using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonM.domain;
using CommonM.domain.config;
using CommonM.domain.vo;
using CommonM.domain.xml;
using CommonM.logger;
using CommonM.util;
using Setting = CommonM.domain.config.Setting;


namespace Upant
{
    class Program
    {
        internal delegate void Func();
        public string backUpDirName => $"bak_{DateTime.Now:yyyyMMdd}";
        static void Main(string[] args)
        {
            LogFactory.initLog();
            Program p = new Program();
            exe(() => p.modifyConfigFile());
        }

        public void modifyConfigFile() {
            string p1 = @"C:\localfile\test\modi.xml";
            string p2 = @"C:\localfile\test\Glorysoft.EAP.Service.exe.config";
            var xm = new XmlFile(p2);
            var um = new UpdateXml(p1);
            ConfigUtil.UpdateConfig(xm, null, "/configuration/appSettings");
            xm.save();
        }
        public void updateXmlTest() {
            string path = @"C:\localfile\test\modi.xml";
            var t = new UpdateXml(path);
            Console.WriteLine(t.ToString());
        }
        public void copyContentTest() {
            string tar = @"C:\localfile\test\local";
            string sou = @"C:\localfile\test\remote";
            var fi = new List<string>() {"eap.config", "Glorysoft.EAP.Service.exe", "Glorysoft.EAP.Service.exe.config" };
            var di = new List<string>() { "logs"};
            FileUtil.copyContent2Dir(sou, tar, fi, di);
        }

        public void delet() {
            string tar = @"C:\localfile\test";
            string tar1 = @"C:\localfile\test\local1";
            FileUtil.copyDirectory(tar1, tar, "local");
        }
        /// <summary>
        /// 备份
        /// </summary>
        public void backupTest() {
            string path = @"C:\localfile\test\local";
            var list = new List<string> { "logs", "Configuration"};
            var files = new List<string> {"RabbitMQ.Client.xml" };
            FileUtil.copyDirectory(path, path, backUpDirName, files, list);
        }
        public  void copyToNowDir() {
            string source = @"C:\localfile\test\error";
            string target = @"C:\localfile\test\proberparam";
            FileUtil.copyContent2Dir(source, target, new List<string>(){"conf.xml"}, null);
        }
        
        public void deserializeTest() {
            string path = @"C:\localfile\test\UpdantConfig.xml";
            var o = (Configuration)ConfigUtil.deserialization(typeof(Configuration), path);
            Console.WriteLine(o.ToString());
        }
        
        public static void exe(Func func)
        {
            var s = new Stopwatch();
            Console.WriteLine("start.....");
            try {
                s.Start();
                func();
                s.Stop();
            }
            catch (Exception e) {
                Console.WriteLine(e);
            }
            Console.WriteLine("end.....");
            Console.WriteLine($"exe run time is {s.ElapsedMilliseconds} ms");
        }
    }
}

