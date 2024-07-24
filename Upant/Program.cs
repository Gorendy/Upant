using System;
using System.Collections.Generic;
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
        static void Main(string[] args)
        {
            LogFactory.initLog();
            Configure conf = new Configure() { setting = new Setting(), config = new SubConfig() };
            /*conf.setting.localPath = @"D:test\ab\aa\ba";
            conf.setting.remotePath = @"C:test001\test";
            conf.setting.configFileName = "test.xml";
            conf.setting.ignores = new List<Ignore>()
            {
                new Ignore(){directories = new List<string>() {"terst", "tst"}, files = new List<string>() {"file", "est02"}}
            };
            conf.config.executeConfigFile = "a.exe.conf";
            conf.config.updateConfigName = "update";
            ConfigUtil.serialization(conf, @"D:\coding\codwork\c#pro\test\test.xml");*/
            string s = @"D:\coding\codwork\c#pro\test\test.xml";
            conf = (Configure) ConfigUtil.deserialization(conf, s);
            Console.WriteLine(conf.ToString());
        }
    }
}

