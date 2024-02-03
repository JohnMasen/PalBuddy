// See https://aka.ms/new-console-template for more information
using System;

Console.WriteLine("Fake server started, press ctrl+C or enter \"exit\" to exit");
Console.CancelKeyPress += Console_CancelKeyPress;
while (true)
{
	if (Console.ReadLine()?.ToLower()=="exit")
	{
		Console.WriteLine("exit entered");
		break;
	}
}


void Console_CancelKeyPress(object? sender, ConsoleCancelEventArgs e)
{
	Console.WriteLine("ctrl+c pressed");
	Environment.Exit(0);
}