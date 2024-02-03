using Bogus;
using KellermanSoftware.CompareNetObjects;
using PalBuddy.Core;
using System.Diagnostics;
using System.Xml.Schema;

namespace UnitTest
{
    public class CoreTests
    {
        private string serverPath = @"C:\GithubRoot\PalBuddy\src\PalBuddy\FakeServer\bin\Debug\net8.0\FakeServer.exe";
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
        public void SaveAndLoad()
        {
            ServerConfig s = new Faker<ServerConfig>()
                .RuleForType<int>(typeof(int), x => x.Random.Int(0, 100))
                .RuleForType<float>(typeof(float), x => x.Random.Float(0.5f, 1.5f))
                .RuleForType<string>(typeof(string), x => x.Lorem.Word())
                .RuleForType<bool>(typeof(bool), x => x.Random.Bool());
            MemoryStream ms = new MemoryStream();
            s.SaveTo(ms,true);
         
            ms.Position = 0;
            ServerConfig s1 = ServerConfig.ReadFrom(ms);

            CompareLogic compareLogic = new CompareLogic();
            compareLogic.Config.MaxDifferences = 99;
            var cr = compareLogic.Compare(s, s1);

            Assert.True(cr.AreEqual);
            if (!cr.AreEqual)
            {
                Debug.WriteLine(cr.DifferencesString);
            }
            
        }

        [Fact]
        public void StartServer()
        {
            PalDedicatedServer server = new PalDedicatedServer(serverPath);
            server.Start();
            Thread.Sleep(2000);//wait for thread start
        }
    }
}