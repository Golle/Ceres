// See https://aka.ms/new-console-template for more information

using Sandbox;
using Titan.Core.Logging;


Logger.Start();
Console.WriteLine("Hello, World!");


var window = new Window("DirectX12 Sandbox", 1024, 768);

while (window.Update())
{

}

Logger.Shutdown();