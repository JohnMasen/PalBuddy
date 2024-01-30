using Bogus;
using PalBuddy.Core;

namespace UnitTest
{
    public class CoreTests
    {
        [Fact]
        public void ReadServerConfigFromFile()
        {
            using var fs = File.OpenRead("ServerConfig.ini");
            var x = ServerConfig.ReadFrom(fs);
        }

        [Fact]
        public void SaveServerConfigToStream()
        {
            MemoryStream ms = new MemoryStream();
            ServerConfig config = new ServerConfig();
            config.SaveTo(ms,true);
            ms.Position = 0;
            StreamReader reader = new StreamReader(ms);
            string s = reader.ReadToEnd();
        }

        [Fact]
        public void Test1()
        {
            ServerConfig s = new Faker<ServerConfig>()
                .RuleForType<int>(typeof(int), x => x.Random.Int(0, 100))
                .RuleForType<float>(typeof(float), x => x.Random.Float(0.5f, 1.5f));
        }
    }
}