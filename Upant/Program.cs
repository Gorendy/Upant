using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonM.logger;
using log4net;
using log4net.Config;
using log4net.Core;


namespace Upant
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("开始 log");
            string configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "conf", "log.xml");
            FileInfo configFile = new FileInfo(configFilePath);
            XmlConfigurator.Configure(configFile);
            Console.WriteLine("log end");
        }
    }
}

