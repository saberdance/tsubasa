using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tsubasa
{
    public static class Logger
    {
        private static string now = string.Format("{0:d}", System.DateTime.Now).Replace('/', '_');
        public static string logfile = ".\\" + now + ".log";
        static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 用于跨天
        /// </summary>
        public static void UpdateLogDatatime()
        {
            now = string.Format("{0:d}", System.DateTime.Now).Replace('/', '_');
            logfile = ".\\" + now + ".log";
        }
        public static void StateOutput(string input)
        {
            Log(input);
            Console.WriteLine($"[STATE]:{input}");
        }
        public static void ConsoleLog(string input)
        {
            Log(input);
            Console.WriteLine($"[{now}]:{input}");
        }
        public static void Log(string input)
        {
            Log("", $"[LOG]{input}");
        }
        public static void Title(string input)
        {
            Log("", $"[LOG]------------------{input}------------------");
        }
        public static void Error(string input)
        {
            Log("", $"[ERROR]{input}");
        }
        public static void Crash(string msg,Exception e,bool needTrace=false)
        {
            Title("CRASH");
            Log("", $"[Crash]:{msg}");
            Log("", $"[Crash Msg]:{e}");
            if (needTrace)
            {
                Log("", $"[Crash Trace:]:{e.StackTrace}");
            }
            EndTag();
        }
        public static void Warning(string input)
        {
            Log("", $"[WARNING]{input}");
        }
        public static void EndTag()
        {
            Log("", "[LOG]------------------------------------------");
        }
        public static void Log(string fileName,string input)
        {
            string logFilePath;
            UpdateLogDatatime();
            if (fileName == "")
            {
                logFilePath = logfile;
            }
            else
            {
                logFilePath = $".\\{fileName}_{now}.log";
            }
             
            try
            {
                LogWriteLock.EnterWriteLock();
                using (StreamWriter log = new StreamWriter(logFilePath, true))
                {
                    //FileStream fs = new FileStream(url, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);FileMode.Append
                    ///设置写数据流的起始位置为文件流的末尾
                    log.BaseStream.Seek(0, SeekOrigin.End);
                    ///写入日志内容并换行
                    log.Write($"\r\n{ DateTime.Now.ToLongTimeString()} : {input}");
                    //清空缓冲区
                    log.Flush();
                    //关闭流
                    log.Close();
                }
            }
            catch (Exception e)
            {

            }
            finally
            {
                LogWriteLock.ExitWriteLock();
            }
        }
    }
}
