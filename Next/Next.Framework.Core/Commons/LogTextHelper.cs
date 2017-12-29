using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Next.Framework.Core.Commons
{
    public class LogTextHelper
    {
        static string LogFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

        public static bool RecordLog = true;
        public static bool DebugLog = true;

        static LogTextHelper()
        {
            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
        }

        public static void WriteLine(string message)
        {
            string temp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]    " + message + "\r\n\r\n");
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                if (RecordLog)
                {
                    File.AppendAllText(Path.Combine(LogFolder, fileName), temp, Encoding.GetEncoding("GB2312"));
                }
                if (DebugLog)
                {
                    Console.WriteLine(temp);
                }
            }
            catch (Exception e)
            {
                throw new Exception("日志记录出错:  " + e.ToString());
            }
        }

        public static void WriteLine(string message, Exception ex)
        {
            string temp = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]    " + message + "\r\n"+ex.ToString()+ "\r\n\r\n");
            string fileName = DateTime.Now.ToString("yyyyMMdd") + ".log";
            try
            {
                if (RecordLog)
                {
                    File.AppendAllText(Path.Combine(LogFolder, fileName), temp, Encoding.GetEncoding("GB2312"));
                }
                if (DebugLog)
                {
                    Console.WriteLine(temp);
                }
            }
            catch (Exception e)
            {
                throw new Exception("日志记录出错:  " + e.ToString());
            }
        }

        public static void WriteLine(string className, string funName, string message)
        {
            WriteLine(string.Format("{0}: {1}\r\n{2}", className, funName, message));
        }
        public static void Debug(object ex)
        {
            WriteLine(ex.ToString());
        }
        public static void Warn(object ex)
        {
            WriteLine(ex.ToString());
        }
        public static void Error(object ex)
        {
            WriteLine(ex.ToString());
        }
        public static void Info(object ex)
        {
            WriteLine(ex.ToString());
        }
        public static void Debug(object message, Exception ex)
        {
            WriteLine(ex.ToString());
        }
        public static void Warn(object message, Exception ex)
        {
            WriteLine(ex.ToString());
        }
        public static void Error(object message, Exception ex)
        {
            WriteLine(ex.ToString());
        }
        public static void Info(object message, Exception ex)
        {
            WriteLine(ex.ToString());
        }
    }
}
