using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalBuddy.Core
{
    public interface IRCONResponse
    {
        public void Parse(string rconResult);
        
    }

    public class UserRecordResponse : IRCONResponse
    {
        public record UserInfo(string Name,int Uid,int SteamID);
        public IEnumerable<UserInfo> Users { get; private set; }
        public void Parse(string rconResult)
        {
            StringReader r = new StringReader(rconResult);
            string? line = r.ReadLine();//skip table header
            line = r.ReadLine();//read first row
            List<UserInfo> info = new List<UserInfo>();
            while (line!=null)
            {
                var parts = line.Split(",");
                info.Add(new UserInfo(parts[0], int.Parse(parts[1]),int.Parse(parts[2])));
                line = r.ReadLine();
            }
            Users = info;
        }
    }
}
