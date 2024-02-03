using Rcon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PalBuddy.Core
{
    public class PalServerRCONClient:IDisposable
    {
        Rcon.RconClient client;
        private bool disposedValue;

        private PalServerRCONClient(RconClient client)
        {
            this.client = client;
        }
        private string runCommand(string command)
        {
            client.SendPacket(new(Rcon.Types.PacketType.SERVERDATA_EXECCOMMAND,command));
            return client.ReceivePacket().Body;
        }
        public string GetInfo()
        {
            return runCommand("Info");
        }
        public string Shutdown(int seconds,string message,bool isForceShutdown=false)
        {
            if (isForceShutdown)
            {
                runCommand("DoExit");
                return string.Empty;
            }
            else
            {
                return runCommand($"Shutdown {seconds} {message}");
            }
        }

        public IEnumerable<UserRecordResponse.UserInfo> GetPlayerList()
        {
            UserRecordResponse r = new UserRecordResponse();
            r.Parse(runCommand("ShowPlayers"));
            return r.Users;
        }

        public void BanPlayer(int steamID)
        {
            runCommand($"BanPlayer {steamID}");
        }

        public void KickPlayer(int steamID)
        {
            runCommand($"KickPlayer {steamID}");
        }

        public string Save()
        {
            return runCommand("Save");
        }


        public static PalServerRCONClient Connect(string addr,int port,string password)
        {
            RconClient client = new Rcon.RconClient();
            client.Connect(addr, port);
            client.Authenticate(password);
            return new PalServerRCONClient(client);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    client.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~PalServerRCONClient()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
