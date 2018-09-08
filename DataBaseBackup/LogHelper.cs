using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;  

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DataBaseBackup
{
    public  class LogHelper
    {
         #region static void WriteLogError(Type t, Exception ex，string msg)

            public static void WriteLogError(Type t, Exception ex, string msg)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                log.Error(msg, ex);
            }

            #endregion

            #region static void WriteLogError(Type t, Exception ex)

            public static void WriteLogError(Type t, Exception ex)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                log.Error("Error", ex);
            }

            #endregion

            #region static void WriteLogInfo(Type t, string msg)

            public static void WriteLogInfo(Type t, string msg)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                log.Info(msg);
            }

            #endregion
            #region static void WriteLogError(Type t, string msg)

            public static void WriteLogError(Type t, string msg)
            {
                log4net.ILog log = log4net.LogManager.GetLogger(t);
                log.Error(msg);
            }

            #endregion
  
    }
}

