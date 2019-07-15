using System.Threading;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using log4net.Core;
using System.Xml;
using System.IO;
using log4net.Config;
using log4net.Repository;

namespace SuperPartner.Utils.Loggers
{
    /// <summary>
    /// Author: Robert
    /// The loger is wrap log4net.
    /// User can easy use Logger.Error("") to log
    /// </summary>
    public class Logger {

        private static ILoggerRepository loggerRepository;
        private static ILog log;

        public static void ConfigurationLog4Net()
        {
            loggerRepository = LogManager.CreateRepository(Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
            XmlConfigurator.ConfigureAndWatch(loggerRepository, new FileInfo("log4net.config"));
            log = LogManager.GetLogger(loggerRepository.Name, typeof(Logger));
        }

        public static void Debug(object message)
        {
            log.Debug(message);
        }

        public static void Info(object message)
        {
            log.Info(message);
        }

        public static void Error(object message)
        {
            log.Error(message);
        }

        public static void Fatal(object message)
        {
            log.Fatal(message);
        }
    }
}
