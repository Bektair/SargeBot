using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using SargeBot.Features.GameData;
using SargeBot.Features.GameInfo;
using SC2APIProtocol;
using SC2ClientApi;
using System;
using System.Diagnostics;
using System.Text;

namespace SargeBot.Tests;

[TestClass]
public class GameClientTests
{
 
    [TestMethod]
    public void testFindFileTime()
    {
        int loops = 100;
        string file = "dataF799E093428D419FD634CCE9B925218C.json";
        Stopwatch stopWatch = new Stopwatch();
        stopWatch.Start();
        for (int i = 0; i < loops; i++)
        {
            GameClient.DataFileExists(file);
        }
        stopWatch.Stop();
        float AvrageTime = stopWatch.ElapsedMilliseconds / loops;
        Console.WriteLine("Avrage for FindFile only " + AvrageTime);
    }


}
