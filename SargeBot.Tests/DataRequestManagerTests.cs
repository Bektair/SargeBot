using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SargeBot.Features.GameData;
using SC2ClientApi;

namespace SargeBot.Tests;

[TestClass]
public class DataRequestManagerTests
{
    private GameClient? _gameClient;
    private GameData? _gameData;
    private IServiceProvider _serviceProvider;

    [TestMethod]
    public void CreatesFileAndDirectoryWithData()
    {
        var dataRequestManager = new DataRequestManager(_gameData);
        var dataFile = "data.json";

        dataRequestManager.CreateFileAndDirectory("data.json");
    }
}