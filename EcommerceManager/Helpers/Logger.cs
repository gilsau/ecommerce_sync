using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;

namespace EcommerceManager.Helpers
{
    public static class Logger
    {
        public static void LogError(Exception ex, List<string> customLines=null) {

            //verify log dir
            string fullLogDirPath = HttpContext.Current.Server.MapPath("/logs/");
            
            //verify log file
            string fullFileName = string.Format("{0}{1:yyyy_MM_dd}.txt", fullLogDirPath, DateTime.Now);
            
            //construct error lines
            List<string> linesPre = new List<string>();
            linesPre.Add("----------------------------------------------------------------------------------");
            linesPre.Add(string.Format("{0:MM/dd/yyyy HH:mm:ss} - {1}", DateTime.Now, Environment.UserName));
            linesPre.Add(ex.Message.ToUpper());
            linesPre.Add("INNER EXCEPTION");
            linesPre.Add(ex.InnerException != null ? ex.InnerException.Message : string.Empty);
            linesPre.Add(string.Empty);
            linesPre.Add("STACK TRACE");
            linesPre.Add(ex.StackTrace);
            linesPre.Add(string.Empty);
            linesPre.Add("SOURCE");
            linesPre.Add(ex.Source);
            linesPre.Add(string.Empty);
            linesPre.Add("TARGET NAME");
            linesPre.Add(ex.TargetSite != null ? ex.TargetSite.Name : string.Empty);
            linesPre.Add(string.Empty);
            linesPre.Add("TARGET MODULE NAME");
            linesPre.Add(ex.TargetSite != null ? ex.TargetSite.Module.Name : string.Empty);
            linesPre.Add(string.Empty);
            if(customLines != null) linesPre.AddRange(customLines);

            //write to file
            File.AppendAllLines(fullFileName, linesPre);
        }

        public static void Log(List<string> customLines)
        {
            //verify log dir
            string fullLogDirPath = HttpContext.Current.Server.MapPath("/logs/");
            
            //verify log file
            string fullFileName = string.Format("{0}{1:yyyy_MM_dd}.txt", fullLogDirPath, DateTime.Now);
            
            //write to file
            customLines.Insert(0, "----------------------------------------------------------------------------------");
            customLines.Insert(1, string.Format("{0:MM/dd/yyyy HH:mm:ss} - {1}", DateTime.Now, Environment.UserName));

            File.AppendAllLines(fullFileName, customLines);
        }
    }
}