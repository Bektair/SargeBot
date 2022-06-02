using Microsoft.VisualStudio.TestTools.UnitTesting;
using SargeBot.Features.GameData;
using SC2ClientApi;
using System;

namespace SargeBotTest;

[TestClass]
public class DataRequestManagerTest
{
    private GameClient? _gameClient;
    private IServiceProvider _serviceProvider;
    private GameData? _gameData;

    [TestMethod]
    public void CreatesFileAndDirectoryWithData()
    {
        DataRequestManager dataRequestManager = new DataRequestManager(_gameClient, _serviceProvider, _gameData);
        string dataFile = "data.json";

        dataRequestManager.CreateFileAndDirectory("data.json");


    }
}
