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
        public event EventHandler<ServerStatusEnum> OnServerStatusChanged;
        private ServerStatusEnum _state = ServerStatusEnum.Idle;
        public string ServerPath { get; private set; }

        public PalDedicatedServer(string serverPath)
        {
            if (string.IsNullOrWhiteSpace(serverPath))
            {
                throw new ArgumentOutOfRangeException("serverPath must be a valid path");
            }
            if (!File.Exists(serverPath))
            {
                throw new FileNotFoundException($"could not find file {serverPath}");
            }
            if (Path.GetFileNameWithoutExtension(serverPath) != "PalServer")
            {
                throw new ArgumentOutOfRangeException("the filename in serverPath must be PalServer[.exe]");
            }
            ServerPath = serverPath;
        }
        public ServerConfig CurrentConfig
        {
            get
            {
                using var s = getSaveFile();
                return ServerConfig.ReadFrom(s);
            }
            set
            {
                using var s = getSaveFile();
                value.SaveTo(s);
            }
        }
        public ServerStatusEnum ServerState
        {
            get
            {
                return _state;
            }
            private set
            {
                if (value != _state)
                {
                    OnServerStatusChanged?.Invoke(this, value);
                }
                _state = value;
            }
        }

        public void Start(bool enablePerformanceOptimization=true)
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
                            UseShellExecute = true,
                            //RedirectStandardInput = true,
                            //CreateNoWindow =false
                        };
                        if (enablePerformanceOptimization)
                        {
                            info.Arguments = "-useperfthreads -NoAsyncLoadingThread -UseMultithreadForDS";
                        }
                        try
                        {
                            serverProcess = Process.Start(info);
                            serverProcess.EnableRaisingEvents = true;
                            serverProcess.Exited += ServerProcess_Exited;
                            ServerState = ServerStatusEnum.Running;
                        }
                        catch (Exception)
                        {
                            ServerState = ServerStatusEnum.Idle;
                            throw;
                        }
                    }
                }
            }
        }


        private Stream getSaveFile()
        {
            string configFolder;
            string serverFolder = Path.GetDirectoryName(ServerPath)!;
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
            return File.Open(configFile, new FileStreamOptions()
            {
                Mode = FileMode.OpenOrCreate,
                Access = FileAccess.ReadWrite,
                Share = FileShare.None
            });

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
