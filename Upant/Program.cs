using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonM.domain.config;
using CommonM.logger;
using CommonM.util;


namespace Upant
{
    class Program
    {
        internal delegate void Func();
        /*static void Main(string[] args)
        {
            LogFactory.initLog();
            exe(() => subDeserializeTest());
        }*/

        public static void subDeserializeTest()
        {
            SubConfigure sub =  (SubConfigure) ConfigUtil.deserialization(typeof(SubConfigure), 
                "C:\\localfile\\test\\subtest.xml");
            Console.WriteLine(sub.ToString());
        }
        public static void subSerializeTest()
        {
            var conf = new SubConfigure(){setting = new SubSetting(), info = new UpdateInfo()};
            conf.setting.fileName = "update.xml";
            conf.setting.exeName = "exe";
            conf.info.nodes = new List<Node>()
            {
                new Node("/pa/p", "A"), new Node("/te", "U")
                ,new Node("/del", "D")
            };
            ConfigUtil.serialization(conf, "C:\\localfile\\test\\subtest.xml");
        }
        public static void deserializeTest()
        {
            Configure conf = new Configure() { setting = new Setting(), config = new SubConfig() };
            string s = @"D:\coding\codwork\c#pro\test\test.xml";
            conf = (Configure) ConfigUtil.deserialization(null, s);
            Console.WriteLine(conf.ToString());
        }
        public static void serializeTest()
        {
            Configure conf = new Configure() { setting = new Setting(), config = new SubConfig() };
            conf.setting.localPath = @"D:test\ab\aa\ba";
            conf.setting.remotePath = @"C:test001\test";
            conf.setting.configFileName = "test.xml";
            
            conf.config.executeConfigFile = "a.exe.conf";
            conf.config.updateConfigName = "update";
            ConfigUtil.serialization(conf, @"D:\coding\codwork\c#pro\test\test.xml");
        }
        public static void exe(Func func)
        {
            var s = new Stopwatch();
            Console.WriteLine("start.....");
            s.Start();
            func();
            s.Stop();
            Console.WriteLine("end.....");
            Console.WriteLine($"exe run time is {s.ElapsedMilliseconds} ms");
        }
    }
}

