using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    //[TestFixture]
    //public class GameLogicTests
    //{
    //    Mock<IManagers> mockIManagers;
    //    IGameSessionFactory sessionFactory;
    //    IGameData gameData;
    //    IGameDataHandler dataHandler;
    //    IGameLogicEx gameLogic;

    //    [SetUp]
    //    public void SetUp()
    //    {
    //        Debug.Log("SetUp");

    //        // Arrange
    //        mockIManagers = new Mock<IManagers>();
    //        sessionFactory = new DefaultGameSessionFactory(mockIManagers.Object);

    //        gameLogic = sessionFactory.CreateGameLogicEx();

    //        // Act
    //        dataHandler.Init();
    //    }

    //    [TearDown]
    //    public void TearDown()
    //    {
    //        Debug.Log("TearDown");

    //        mockIManagers = null;
    //        sessionFactory = null;
    //        gameData = null;
    //        dataHandler = null;
    //        gameLogic = null;
    //    }

    //    [Test]
    //    public void DataHandler_Should_Init_GameData()
    //    {
    //        // Assert
    //        Assert.IsNotNull(gameData.Players);
    //        Assert.IsTrue(gameData.Players.Length == 2);
    //        Assert.IsNotNull(gameData.Players[0]);
    //    }

    //    [Test]
    //    public void DataHandler_RegisterPlayerDeck_Should_Register()
    //    {
    //        // Arrange
    //        // 30장 짜리 덱 만들자. List<string> string은 soID
    //        List<string> deck1 = new List<string>
    //        {
    //            "Yuas7GOx", "FCgx9bb5", "viiQ4nPX", "4RVdVPHv", "18KQdgnF",
    //            "bmhDtP5S", "cQpRMzQm", "SKC3xLlD", "NQhInLB6", "dlqm3oXy",
    //            "zGeBtJeq", "G5QGWS5X", "ZnZsgItK", "rHsEk7ES", "96oXyqZA",
    //            "fWxrKWg7", "0V6yENjO", "WjFJzbNz", "Az7cHL03", "PhTwMe0A",
    //            "q5CujQug", "KXUKvJOK", "ZigqPIIQ", "qna4Lwi1", "qSPTPRbU",
    //            "qKkH9cJa", "poHkt3oT", "u7Q7pWS7", "VzqN9iGO", "lkZVF35D"
    //        };

    //        List<string> deck2 = new List<string>
    //        {
    //            "mAlCr45z", "Id2hPsuy", "xTHWEXeT", "oTtBFsHi", "Rrsdy8LG",
    //            "5WiriKan", "ieJSH67J", "8S66Yi4R", "6hySxoSz", "W4lyp4bo",
    //            "phkU1voh", "GBxKd2a6", "2uhv8gDy", "hgSOHsVn", "Ab3bgkNJ",
    //            "np7hJTra", "MTLeELNv", "VXT8YGIk", "MzGHA8yF", "zrUycSui",
    //            "BZFJwgEr", "MvOKZwJX", "8ZxUgUvS", "sqPB5Qgo", "9suIqoBf",
    //            "hZdzUUP1", "sSmMQwAq", "5pGyZ5ug", "85vrp8n2", "TDvOjxr9"
    //        };

    //        // Act
    //        dataHandler.RegisterPlayerDeck(0, deck1);
    //        dataHandler.RegisterPlayerDeck(1, deck2);

    //        dataHandler.InitPlayers();

    //        // Assert
    //        // 게임 데이터에 반영이 되었는지?
    //        Assert.IsNotNull(gameData.Players[0]);
    //        Assert.IsNotNull(gameData.Players[1]);
    //        Assert.IsNotNull(gameData.Players[0].Deck);
    //        Assert.IsNotNull(gameData.Players[1].Deck);

    //        Assert.IsTrue(gameData.Players[0].Deck.Count == 30);
    //        Assert.IsTrue(gameData.Players[1].Deck.Count == 30);
    //    }

    //    [Test]
    //    public void GameLogic_StartGame_Should_SetPlayers()
    //    {
    //        // Arrange
    //        var mockDataHandler = new Mock<IGameDataHandler>();
    //        var tempGameLogic = new DefaultGameLogic();

    //        var players = new Player[0];
    //        mockDataHandler
    //            .Setup(Handler => Handler.GameData.Players)
    //            .Returns(players);

    //        var tempPlayer = new Player();
    //        mockDataHandler
    //            .Setup(Handler => Handler.GetOpponentPlayer(It.IsAny<ulong>()))
    //            .Returns(tempPlayer);

    //        var mockPlayer = new Player();
    //        mockPlayer.SetPlayerID(1);
    //        mockDataHandler
    //            .Setup(Handler => Handler.CurrentPlayer)
    //            .Returns(mockPlayer);

    //        var expectedCard = new Card();
    //        mockDataHandler
    //            .Setup(Handler => Handler.DrawCard(It.IsAny<Player>(), out expectedCard));

    //        tempGameLogic.StartGame();

    //        // Assert
    //        mockDataHandler.Verify(handler => handler.InitPlayers(), Times.Once, "SetPlayers should be called exactly once.");
    //    }

    //    [Test]
    //    public void GameLogic_StartGame_Should_Register()
    //    {
    //        // Arrange
    //        // 30장 짜리 덱 만들자. List<string> string은 soID
    //        List<string> deck1 = new List<string>
    //        {
    //            "Yuas7GOx", "FCgx9bb5", "viiQ4nPX", "4RVdVPHv", "18KQdgnF",
    //            "bmhDtP5S", "cQpRMzQm", "SKC3xLlD", "NQhInLB6", "dlqm3oXy",
    //            "zGeBtJeq", "G5QGWS5X", "ZnZsgItK", "rHsEk7ES", "96oXyqZA",
    //            "fWxrKWg7", "0V6yENjO", "WjFJzbNz", "Az7cHL03", "PhTwMe0A",
    //            "q5CujQug", "KXUKvJOK", "ZigqPIIQ", "qna4Lwi1", "qSPTPRbU",
    //            "qKkH9cJa", "poHkt3oT", "u7Q7pWS7", "VzqN9iGO", "lkZVF35D"
    //        };

    //        List<string> deck2 = new List<string>
    //        {
    //            "mAlCr45z", "Id2hPsuy", "xTHWEXeT", "oTtBFsHi", "Rrsdy8LG",
    //            "5WiriKan", "ieJSH67J", "8S66Yi4R", "6hySxoSz", "W4lyp4bo",
    //            "phkU1voh", "GBxKd2a6", "2uhv8gDy", "hgSOHsVn", "Ab3bgkNJ",
    //            "np7hJTra", "MTLeELNv", "VXT8YGIk", "MzGHA8yF", "zrUycSui",
    //            "BZFJwgEr", "MvOKZwJX", "8ZxUgUvS", "sqPB5Qgo", "9suIqoBf",
    //            "hZdzUUP1", "sSmMQwAq", "5pGyZ5ug", "85vrp8n2", "TDvOjxr9"
    //        };

    //        // Act
    //        dataHandler.RegisterPlayerDeck(0, deck1);
    //        dataHandler.RegisterPlayerDeck(1, deck2);

    //        gameLogic.StartGame();

    //        // Assert
    //        // 게임 데이터에 반영이 되었는지?
    //        Assert.IsNotNull(gameData.Players[0]);
    //        Assert.IsNotNull(gameData.Players[1]);
    //        Assert.IsNotNull(gameData.Players[0].Deck);
    //        Assert.IsNotNull(gameData.Players[1].Deck);

    //        Assert.IsTrue(gameData.Players[0].Deck.Count == 26);
    //        Assert.IsTrue(gameData.Players[1].Deck.Count == 26);

    //        dataHandler.DrawCard(dataHandler.GetPlayer(0), out _);

    //        Assert.IsTrue(gameData.Players[0].Deck.Count == 25);
    //    }

    //    [Test]
    //    public void GameLogic_StartGame_Should_Inovke_OnDataUpdated()
    //    {
    //        // Arrange
    //        // 30장 짜리 덱 만들자. List<string> string은 soID
    //        List<string> deck1 = new List<string>
    //        {
    //            "Yuas7GOx", "FCgx9bb5", "viiQ4nPX", "4RVdVPHv", "18KQdgnF",
    //            "bmhDtP5S", "cQpRMzQm", "SKC3xLlD", "NQhInLB6", "dlqm3oXy",
    //            "zGeBtJeq", "G5QGWS5X", "ZnZsgItK", "rHsEk7ES", "96oXyqZA",
    //            "fWxrKWg7", "0V6yENjO", "WjFJzbNz", "Az7cHL03", "PhTwMe0A",
    //            "q5CujQug", "KXUKvJOK", "ZigqPIIQ", "qna4Lwi1", "qSPTPRbU",
    //            "qKkH9cJa", "poHkt3oT", "u7Q7pWS7", "VzqN9iGO", "lkZVF35D"
    //        };

    //        List<string> deck2 = new List<string>
    //        {
    //            "mAlCr45z", "Id2hPsuy", "xTHWEXeT", "oTtBFsHi", "Rrsdy8LG",
    //            "5WiriKan", "ieJSH67J", "8S66Yi4R", "6hySxoSz", "W4lyp4bo",
    //            "phkU1voh", "GBxKd2a6", "2uhv8gDy", "hgSOHsVn", "Ab3bgkNJ",
    //            "np7hJTra", "MTLeELNv", "VXT8YGIk", "MzGHA8yF", "zrUycSui",
    //            "BZFJwgEr", "MvOKZwJX", "8ZxUgUvS", "sqPB5Qgo", "9suIqoBf",
    //            "hZdzUUP1", "sSmMQwAq", "5pGyZ5ug", "85vrp8n2", "TDvOjxr9"
    //        };

    //        dataHandler.RegisterPlayerDeck(0, deck1);
    //        dataHandler.RegisterPlayerDeck(1, deck2);

    //        gameLogic.OnDataUpdated += () =>
    //        {
    //            Debug.Log("Data Update");
    //        };

    //        gameLogic.OnGameStarted += () =>
    //        {
    //            Debug.Log("Game Start");
    //        };

    //        // Act
    //        gameLogic.StartGame();

    //        // Assert
    //        LogAssert.Expect(LogType.Log, "Data Update");
    //        LogAssert.Expect(LogType.Log, "Game Start");
    //    }

    //    [Test]
    //    public void JsonSerializeTest()
    //    {
    //        // 30장 짜리 덱 만들자. List<string> string은 soID
    //        List<string> deck1 = new List<string>
    //        {
    //            "Yuas7GOx", "FCgx9bb5", "viiQ4nPX", "4RVdVPHv", "18KQdgnF"
    //        };

    //        List<string> deck2 = new List<string>
    //        {
    //            "mAlCr45z", "Id2hPsuy", "xTHWEXeT", "oTtBFsHi", "Rrsdy8LG"
    //        };

    //        dataHandler.RegisterPlayerDeck(0, deck1);
    //        dataHandler.RegisterPlayerDeck(1, deck2);
    //        gameLogic.StartGame();

    //        var serializedJson = NetworkTool.JsonSerialize(dataHandler.GameData);

    //        var utfBytes = Encoding.UTF8.GetBytes(serializedJson);
    //        string deserializedJson = Encoding.UTF8.GetString(utfBytes);

    //        //Debug.Log(deserializedJson);

    //        var deserializedGameData = NetworkTool.JsonDeserialize<DefaultGameData>(deserializedJson);

    //        var secondSerializedJson = NetworkTool.JsonSerialize(deserializedGameData);
    //        Debug.Log(secondSerializedJson);

    //        LogAssert.Expect(LogType.Log, serializedJson);
    //    }

    //    [Test]
    //    public void NetworkSerializeTest()
    //    {
    //        // 30장 짜리 덱 만들자. List<string> string은 soID
    //        List<string> deck1 = new List<string>
    //        {
    //            "Yuas7GOx", "FCgx9bb5", "viiQ4nPX", "4RVdVPHv", "18KQdgnF",
    //            "bmhDtP5S", "cQpRMzQm", "SKC3xLlD", "NQhInLB6", "dlqm3oXy",
    //            "zGeBtJeq", "G5QGWS5X", "ZnZsgItK", "rHsEk7ES", "96oXyqZA",
    //            "fWxrKWg7", "0V6yENjO", "WjFJzbNz", "Az7cHL03", "PhTwMe0A",
    //            "q5CujQug", "KXUKvJOK", "ZigqPIIQ", "qna4Lwi1", "qSPTPRbU",
    //            "qKkH9cJa", "poHkt3oT", "u7Q7pWS7", "VzqN9iGO", "lkZVF35D"
    //        };

    //        List<string> deck2 = new List<string>
    //        {
    //            "mAlCr45z", "Id2hPsuy", "xTHWEXeT", "oTtBFsHi", "Rrsdy8LG",
    //            "5WiriKan", "ieJSH67J", "8S66Yi4R", "6hySxoSz", "W4lyp4bo",
    //            "phkU1voh", "GBxKd2a6", "2uhv8gDy", "hgSOHsVn", "Ab3bgkNJ",
    //            "np7hJTra", "MTLeELNv", "VXT8YGIk", "MzGHA8yF", "zrUycSui",
    //            "BZFJwgEr", "MvOKZwJX", "8ZxUgUvS", "sqPB5Qgo", "9suIqoBf",
    //            "hZdzUUP1", "sSmMQwAq", "5pGyZ5ug", "85vrp8n2", "TDvOjxr9"
    //        };

    //        dataHandler.RegisterPlayerDeck(0, deck1);
    //        dataHandler.RegisterPlayerDeck(1, deck2);
    //        gameLogic.StartGame();

    //        var sdata = new SerializedGameData();
    //        sdata.GameData = dataHandler.GameData;

    //        byte[] serializedBytes = NetworkTool.Serialize(sdata.GameData);
    //        IGameData deserializedGameData = NetworkTool.Deserialize<DefaultGameData>(serializedBytes);

    //        // Assert
    //        Assert.IsNotNull(serializedBytes, "Serialized bytes should not be null.");
    //        Assert.IsNotNull(deserializedGameData, "Deserialized GameData should not be null.");
    //        Assert.AreEqual(dataHandler.GameData.TurnCount, deserializedGameData.TurnCount, "TurnCount should match.");
    //        Assert.AreEqual(NetworkTool.JsonSerialize(dataHandler.CurrentPlayer), NetworkTool.JsonSerialize(deserializedGameData.CurrentPlayer), "CurrentPlayer should match.");
    //    }
    //}
}