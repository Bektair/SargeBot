using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SargeBot.Features.GameData;
using SC2ClientApi;

namespace SargeBot.Tests;

[TestClass]
public class DataRequestManagerTests
{
    private GameClient? _gameClient;
    private GameDataService? _gameData;
    private IServiceProvider _serviceProvider;

    [TestMethod]
    public void AvrageDataLoadTime()
    {
        DataRequestManager man = new DataRequestManager(_gameData);
        int loops = 1;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        for(int i = 0; i < loops; i++) { 
            man.LoadDataFullPath(@"C:\Users\oyvin\source\repos\SargeBot\SargeBot\bin\Debug\net6.0\data\dataF799E093428D419FD634CCE9B925218C.json");
        }
        stopWatch.Stop();
        float AvrageTime = stopWatch.ElapsedMilliseconds/loops;
        Console.WriteLine("Avrage for load only" + AvrageTime);
    }

  



}