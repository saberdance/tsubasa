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
        public static string logdir = "./logs";
        public static string logfile = $"{logdir}/{now}.log";
        private static bool logdirExists = false;
        static ReaderWriterLockSlim LogWriteLock = new ReaderWriterLockSlim();
        /// <summary>
        /// 用于跨天
        /// </summary>
        public static void UpdateLogDatatime()
        {
            now = string.Format("{0:d}", System.DateTime.Now).Replace('/', '_');
            logfile = $"{logdir}/{now}.log";
        }

        public static void InitLogFile(string fileName)
        {
            string logFilePath = $"{logdir}/{fileName}_{now}.log";
            if (File.Exists(logFilePath))
            {
                try
                {
                    File.Delete(logFilePath);
                }
                catch (Exception)
                {
                }
            }
        }

        public static void Log<T>(string input,bool consoleOutput = false)
        {
            Log("", $"[LOG][{typeof(T).Name}]{input}");
            if (consoleOutput)
            {
                Console.WriteLine($"[{now}][{typeof(T)}]:{input}");
            }
        }

        public static void Error<T>(string input, bool consoleOutput = false)
        {
            Log("", $"[Error][{typeof(T).Name}]{input}");
            if (consoleOutput)
            {
                Console.WriteLine($"[{now}][{typeof(T)}]:{input}");
            }
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
        //为了照顾以前的代码，这里加入了一个写的非常丑的目录存在性判断，需要修正
        public static void Log(string fileName,string input)
        {
            if (!logdirExists)
            {
                try
                {
                    if (!Directory.Exists(logdir))
                    {
                        Directory.CreateDirectory(logdir);
                        logdirExists = true;
                    }
                }
                catch (Exception)
                {
                }               
            }                  
            string logFilePath;
            UpdateLogDatatime();
            if (fileName == "")
            {
                logFilePath = logfile;
            }
            else
            {
                logFilePath = $"{logdir}/{fileName}_{now}.log";
            }
             
            try
            {
                LogWriteLock.EnterWriteLock();
                using (StreamWriter log = new StreamWriter(logFilePath, true))
                {
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
            catch (Exception)
            {

            }
            finally
            {
                LogWriteLock.ExitWriteLock();
            }
        }
    }
}