using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace tsubasa
{
    public class NamedPipeServer
    {
        NamedPipeServerStream pipeServer = null;
        string pipeName = "";
        Queue<string> inBuffer = new Queue<string>();
        #region (Initialize Funcs...)
        private NamedPipeServer(string name)
        {
            pipeName = name;
        }

        private bool Init(string Name)
        {
            if (pipeServer != null)
            {
                Logger.Error("[Server]NamedPipe Already Initialized");
                return false;
            }
            try
            {
#pragma warning disable CA1416 // 验证平台兼容性
                pipeServer = new NamedPipeServerStream(Name, PipeDirection.In, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
#pragma warning restore CA1416 // 验证平台兼容性
                Logger.Log($"[Server]Pipe Server:[{Name}]Created");
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash("[Server]Create Pipe Server", e, true);
                return false;
            }
        }
        #endregion
        public static NamedPipeServer CreateLocalServer(string name)
        {
            NamedPipeServer pipe = new NamedPipeServer(name);
            return pipe.Init(name) is true ? pipe : null;
        }

        public bool StartSyncConnection()
        {
            Logger.Log("[Server]Start Sync Connection");
            ThreadPool.QueueUserWorkItem(delegate
            {
                pipeServer.BeginWaitForConnection((o) =>
                {
                    NamedPipeServerStream pServer = (NamedPipeServerStream)o.AsyncState;
                    pServer.EndWaitForConnection(o);
                    Logger.Log("Client Connected");
                    StreamReader sr = new StreamReader(pServer);
                    string line = null;
                    while (true)
                    {
                        line = sr.ReadLine();
                        if (line is not null)
                        {
                            Logger.Log($"[Server]Read Message:{line}");
                            inBuffer.Enqueue(line);
                        }
                        Thread.Sleep(300);
                    }
                }, pipeServer);
            });
            return true;
        }
        public string GetInBuffer()
        {
            return inBuffer.Count > 0 ? inBuffer.Dequeue() : string.Empty;
        }
    }

    public class NamedPipeClient
    {
        NamedPipeClientStream pipeClient = null;
        string pipeName = "";
        StreamWriter sw = null;
        #region (Initialize Funcs...)
        private NamedPipeClient(string Name)
        {
            pipeName = Name;
        }

        private bool Init(string Name)
        {
            if (pipeClient is not null)
            {
                Logger.Error("NamedPipe Already Initialized");
                return false;
            }
            try
            {
                pipeClient = new NamedPipeClientStream(".", Name, PipeDirection.Out, PipeOptions.Asynchronous, TokenImpersonationLevel.None);
                Logger.Log("Pipe Client: "+Name+" Created");
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash("Create Pipe Client", e, true);
                return false;
            }
        }
        #endregion
        public static NamedPipeClient CreateLocalPipe(string name)
        {
            NamedPipeClient pipe = new NamedPipeClient(name);
            return pipe.Init(name) is true ? pipe : null;
        }

        public bool Connect()
        {
            try
            {
                pipeClient.Connect(5000);
                Logger.Log("[Client]Pipe Client Connected");
                sw = new StreamWriter(pipeClient);
                sw.AutoFlush = true;
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash("[Client]Pipe Client Connect", e, true);
                return false;
            }
        }
        public bool Write(string msg,Encoding encoding=null)
        {
            try
            {
                if (pipeClient.IsConnected is false)
                {
                    Logger.Log("[Client]Pipe Client Not Connected:");
                    return false;
                }
                Logger.Log($"[Client]Pipe Client Write:{msg}");
                sw.Write(msg);
                return true;
            }
            catch (Exception e)
            {
                Logger.Crash("[Client]Pipe Client Write", e, true);
                return false;
            }
        }
    }
}
