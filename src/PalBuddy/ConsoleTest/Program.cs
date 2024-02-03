// See https://aka.ms/new-console-template for more information
using PalBuddy.Core;

string serverPath = @"C:\GithubRoot\PalBuddy\src\PalBuddy\FakeServer\bin\Debug\net8.0\FakeServer.exe";
var x=Path.GetDirectoryName(serverPath);
PalBuddy.Core.PalDedicatedServer server = new PalBuddy.Core.PalDedicatedServer(serverPath);
//server.Start();
//Console.ReadLine();
//server.Kill();
PalBuddy.Core.PalServerRCONClient rCONClient = PalServerRCONClient.Connect("192.168.0.18", 25575, "abc");
Console.WriteLine($"getinfo={rCONClient.GetInfo()}");
Console.WriteLine($"$shutdown ={rCONClient.Shutdown(10, null)}");