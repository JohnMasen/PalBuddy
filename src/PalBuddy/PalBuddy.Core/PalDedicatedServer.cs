using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Enumeration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PalBuddy.Core
{
    public enum ServerStatusEnum
    {
        Running,
        Idle,
        Starting,
        Stopping,
        Killing
    }


    public class PalDedicatedServer
    {
        Process serverProcess = null;
        object syncRoot = new object();
        public event EventHandler<ServerStatusEnum> ServerStatusChanged;
        private ServerStatusEnum _state=ServerStatusEnum.Idle;
        public string ServerPath { get; private set; }

        public PalDedicatedServer(string serverPath)
        {
            ServerPath = serverPath;
        }
        public ServerConfig CurrentConfig { get; private set; }
        public ServerStatusEnum ServerState 
        { 
            get
            {
                return _state;
            }
            private set
            {
                if (value!=_state)
                {
                    ServerStatusChanged?.Invoke(this, value);
                }
                _state = value;
            }
        }
        
        public ServerConfig? Start()
        {
            if (ServerState == ServerStatusEnum.Idle)
            {
                lock (syncRoot)
                {
                    if (ServerState == ServerStatusEnum.Idle)
                    {
                        ServerState = ServerStatusEnum.Starting;
                        ProcessStartInfo info = new ProcessStartInfo()
                        {
                            FileName = ServerPath,
                            //UseShellExecute = true,
                            RedirectStandardInput = true,
                            //CreateNoWindow =false
                        };
                        try
                        {
                            serverProcess = Process.Start(info);
                            ServerState = ServerStatusEnum.Running;
                            return loadConfig();
                        }
                        catch (Exception)
                        {
                            ServerState = ServerStatusEnum.Idle;
                            throw;
                        }
                    }
                }
                serverProcess.Exited += ServerProcess_Exited;
            }
            return null;
        }

        private ServerConfig loadConfig()
        {
            string configFolder = null;
            string serverFolder = Path.GetDirectoryName(ServerPath);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                configFolder = Path.Combine(serverFolder, "Pal", "Saved", "Config", "LinuxServer");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                configFolder = Path.Combine(serverFolder, "Pal", "Saved", "Config", "WindowsServer");
            }
            else
            {
                throw new NotSupportedException("Current OS does is not supported by palworld dedicated server");
            }
            if (!Directory.Exists(serverFolder))
            {
                Directory.CreateDirectory(serverFolder);
            }
            string configFile = Path.Combine(configFolder, "PalWorldSettings.ini");
            if (File.Exists(configFile))
            {
                return ServerConfig.ReadFrom(configFile);
            }
            else
            {
                return new ServerConfig();
            }
        }

        private void ServerProcess_Exited(object? sender, EventArgs e)
        {
            serverProcess.Exited -= ServerProcess_Exited;
            serverProcess = null;
            ServerState = ServerStatusEnum.Idle;
        }

        //public void Stop()
        //{
        //    if (ServerState==ServerStatusEnum.Running)
        //    {
        //        lock (syncRoot)
        //        {
        //            if (ServerState==ServerStatusEnum.Running)
        //            {
        //                ServerState = ServerStatusEnum.Stopping;
        //                //serverProcess.CloseMainWindow();
        //                serverProcess.StandardInput.Write("\x3");//send ctrl+c
        //                //serverProcess.WaitForExit(TimeSpan.FromSeconds(60));
        //                //no need set server state to idle, this is done by Process_Exited handler
        //            }
        //        }
        //    }
        //}

        public void Kill()
        {
            if (ServerState == ServerStatusEnum.Running)
            {
                lock (syncRoot)
                {
                    if (ServerState == ServerStatusEnum.Running)
                    {
                        serverProcess.Kill();
                    }
                }
            }
        }
    }
}
