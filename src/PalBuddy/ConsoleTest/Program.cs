// See https://aka.ms/new-console-template for more information
using PalBuddy.Core;

//string serverPath = Path.Combine( Environment.CurrentDirectory,"PalServer.exe");
//var x=Path.GetDirectoryName(serverPath);
//PalBuddy.Core.PalDedicatedServer server = new PalBuddy.Core.PalDedicatedServer(serverPath);
//server.OnServerStatusChanged += (_,e) => Console.WriteLine(e);


//var cfg = server.CurrentConfig;
//cfg.RCONEnabled = true;
//cfg.RCONPort = 25575;
//cfg.AdminPassword = "abc";
//server.CurrentConfig = cfg;
//server.Start();
//Console.ReadLine();
//server.Kill();
PalServerRCONClient rCONClient=null;
for (int i = 0; i < 5; i++)
{
	try
	{
        rCONClient = PalServerRCONClient.Connect("192.168.0.18", 25575, "abc");
    }
	catch (Exception ex)
	{
		Console.WriteLine("connection failed,retry in 1 seconds");
		await Task.Delay(TimeSpan.FromSeconds(1));
	}

}

Console.WriteLine($"getinfo={rCONClient.GetInfo()}");

//foreach (var item in rCONClient.GetPlayerList())
//{
//    Console.WriteLine(item);
//}

Console.WriteLine($"$shutdown ={rCONClient.Shutdown(10, null)}");
Console.WriteLine("press ENTER to exit");
Console.ReadLine();