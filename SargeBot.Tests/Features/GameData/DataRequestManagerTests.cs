using System;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SargeBot.Features.GameData;
using SargeBot.Options;
using SC2ClientApi;

namespace SargeBot.Tests;

[TestClass]
public class DataRequestManagerTests
{
    private GameClient? _gameClient;
    private IServiceProvider _serviceProvider;
    private CacheOptions options;

    private void setUPSerivces()
    {
        var serviceCollection = new ServiceCollection();
        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    [TestMethod]
    public void AvrageDataLoadTime()
    {
        setUPSerivces();
        int loops = 1;
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        for(int i = 0; i < loops; i++) { 
            //man.LoadDataFullPath(@"C:\Users\oyvin\source\repos\SargeBot\SargeBot\bin\Debug\net6.0\data\dataF799E093428D419FD634CCE9B925218C.json");
        }
        stopWatch.Stop();
        float AvrageTime = stopWatch.ElapsedMilliseconds/loops;
        Console.WriteLine("Avrage for load only" + AvrageTime);
    }

  



}