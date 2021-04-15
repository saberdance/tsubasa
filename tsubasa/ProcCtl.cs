using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsubasa
{
    public static class ProcCtl
    {
        /// <summary>
        /// 根据“精确进程名”结束进程
        /// </summary>
        /// <param name="strProcName">精确进程名</param>
        public static void KillProc(string strProcName)
        {
            Logger.Log($"Try Kill:{strProcName}");
            //精确进程名  用GetProcessesByName
            foreach (Process p in Process.GetProcessesByName(strProcName))
            {
                if (!p.CloseMainWindow())
                {
                    p.Kill();
                }
            }
        }
        /// <summary>
        /// 根据 模糊进程名 结束进程
        /// </summary>
        /// <param name="strProcName">模糊进程名</param>
        public static void KillProcA(string strProcName)
        {
            //模糊进程名  枚举
            Logger.Log($"Try KillA:{strProcName}");
            foreach (Process p in Process.GetProcesses())
            {
                if (p.ProcessName.IndexOf(strProcName) > -1)  //第一个字符匹配的话为0，这与VB不同
                {
                    if (!p.CloseMainWindow())
                    {
                        p.Kill();
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否包含此字串的进程   模糊
        /// </summary>
        /// <param name="strProcName">进程字符串</param>
        /// <returns>是否包含</returns>
        public static bool SearchProcA(string strProcName)
        {
            try
            {
                //模糊进程名  枚举
                return Process.GetProcesses().ToList().Exists(p => p.ProcessName.IndexOf(strProcName) > -1);
            }
            catch
            {
                return false;
            }
        }


        /// <summary>
        /// 判断是否存在进程  精确
        /// </summary>
        /// <param name="strProcName">精确进程名</param>
        /// <returns>是否包含</returns>
        public static bool SearchProc(string strProcName)
        {
            try
            {
                //精确进程名  用GetProcessesByName
                return Process.GetProcessesByName(strProcName).Length > 0;
            }
            catch
            {
                return false;
            }
        }
        public static Process StartProc(string strProcPath) 
        {
            Process P;
            try
            { 
                P=Process.Start(strProcPath);
            }
            catch
            {
               Logger.Log($"Start Proc Failed:{strProcPath}");
               return null;
            }
            return P;
        }

        public static void StartRedirectIOProc(string exePath, string arguments, out string output, out string error)
        {
            using (Process process = new System.Diagnostics.Process())
            {
                process.StartInfo.FileName = exePath;
                process.StartInfo.Arguments = arguments;
                // 必须禁用操作系统外壳程序  
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
                process.WaitForExit();
                process.Close();
            }

        }

        public static Process StartSilentProc(string strProcPath, string args)
        {
            Process myProcess = new Process();
            try
            {
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = strProcPath;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.CreateNoWindow = true;
                // myProcess.EnableRaisingEvents = true;
                myProcess.StartInfo.Arguments = args;
                myProcess.Start();
                //myProcess.WaitForExit();
            }
            catch (Exception e)
            {
                Logger.Log($"Start Silent Proc: {strProcPath} Failed.Reseaon:{e.Message}");
            }
            return myProcess;
        }

        public static Process StartProcAndWait(string strProcPath, string args)
        {
            Process myProcess = new Process();
            try
            {
                myProcess.StartInfo.WorkingDirectory = Path.GetDirectoryName(strProcPath);
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.FileName = strProcPath;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.CreateNoWindow = true;
                // myProcess.EnableRaisingEvents = true;
                myProcess.StartInfo.Arguments = args;
                myProcess.Start();
                myProcess.WaitForExit();
            }
            catch (Exception e)
            {
                Logger.Log($"Start Silent Proc: {strProcPath} Failed.Reseaon:{e.Message}");
            }
            return myProcess;
        }

        public static List<int> GetProcessID(string procName)
        {
            Process[] ps = Process.GetProcessesByName(procName);
            if (ps.Length>0)
            {
                return ps.Select(p => p.Id).ToList();
            }
            return null;
        }
        public static void RunCmdAsync(string cmd, DataReceivedEventHandler outputReciver, DataReceivedEventHandler errorReciver = null,EventHandler finishHandler = null)
        {
            Process proc = new Process();
            System.Console.OutputEncoding = System.Text.Encoding.Default;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.WorkingDirectory = ".";
            //proc.StartInfo.Arguments = cmd;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.OutputDataReceived += outputReciver;
            if (errorReciver != null)
            {
                proc.ErrorDataReceived += errorReciver;
                proc.StartInfo.RedirectStandardError = true;
            }                         
            if (finishHandler != null)
            {
                proc.EnableRaisingEvents = true;
                proc.Exited += (finishHandler);
            }
            proc.Start();
            proc.StandardInput.WriteLine(cmd);
            //proc.StandardInput.WriteLine("exit");
            proc.BeginOutputReadLine();
            if (errorReciver != null)
            {
                proc.BeginErrorReadLine();
            }
        }

        public static void RunProcAsync(string procPath,string args, out BinaryReader outputReciver, out BinaryReader errorReciver, EventHandler finishHandler)
        {
            Process proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = procPath;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.EnableRaisingEvents = true;
            proc.Exited += (finishHandler);
            proc.Start();
            outputReciver = new BinaryReader(proc.StandardOutput.BaseStream);
            errorReciver = new BinaryReader(proc.StandardError.BaseStream);
        }
    }
}

