using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebFile.Models
{
    
    public class LogHelper
    {
        public static readonly log4net.ILog LogIngo = log4net.LogManager.GetLogger("LogInfo");
        public static readonly log4net.ILog LogError = log4net.LogManager.GetLogger("Error");
        static LogHelper()
        {
            SetConfig();
        }

        public static void SetConfig()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public static void SetConfig(FileInfo configFileInfo)
        {
            log4net.Config.XmlConfigurator.Configure(configFileInfo);
        }

        public static void WriteLog(string message)
        {
            if (LogIngo.IsInfoEnabled)
            {
                LogIngo.Info(message);
            }
        }
        public static void WriteLog(string message,Exception e)
        {
            if (LogError.IsErrorEnabled)
            {
                LogError.Error(message,e);
            }
        }
    }
}