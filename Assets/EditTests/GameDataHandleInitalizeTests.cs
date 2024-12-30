using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameDataHandleInitalizeTests
    {
        DefaultGameDataHandler dataHandler;
        Mock<IGameLogic> mockManagers;
        Mock<IDataManager> mockDataManager;
        List<PlayerInfo> playerInfos;
        Mock<IGameLogicConfig> mockConfig;
        DefaultGameDataHandlerFactory handlerFactory;

        string expectedID;

        [SetUp]
        public void SetUp()
        {
            expectedID = "HIHELLOEVERYONE";

            dataHandler = new DefaultGameDataHandler();

            mockManagers = new Mock<IGameLogic>();
            mockDataManager = new Mock<IDataManager>();
            
            mockManagers
                .Setup(managers => managers.Data)
                .Returns(mockDataManager.Object);
            var mockDictionary = new Mock<IDictionary<string, CardSO>>();

            mockDataManager
                .Setup(data => data.GetDictionary<CardSO>())
                .Returns(mockDictionary.Object);

            var tempCardSO = ScriptableObject.CreateInstance<CardSO>();
            tempCardSO.SetID(expectedID);

            mockDictionary
                .Setup(dictionary => dictionary.TryGetValue(It.IsAny<string>(), out tempCardSO))
                .Returns(true);   

            playerInfos = new List<PlayerInfo>();

            var host = new PlayerInfo();
            host.ClientID = 0;
            
            host.Deck = new();
            host.Deck.Add(expectedID);

            var client = new PlayerInfo();
            client.ClientID = 1;
            client.Deck = new();

            playerInfos.Add(client);
            playerInfos.Add(host);

            mockConfig = new Mock<IGameLogicConfig>();
            mockConfig
                .Setup(conf => conf.CountOfPlayer)
                .Returns(2);

            handlerFactory = new DefaultGameDataHandlerFactory(mockManagers.Object, mockConfig.Object, playerInfos);

            dataHandler.Init(handlerFactory);
        }

        [SetUp]
        public void TearDown()
        {

        }

        [Test]
        public void Init_Should_GameData_PlayersCount_Set_ConfigPlayersCount()
        {
            Assert.IsNotNull(dataHandler.GameData);
            Assert.IsTrue(dataHandler.GameData.Players.Length == 2);
        }

        [Test]
        public void Init_Should_SetPlayer()
        {
            Assert.IsNotNull(dataHandler.GameData.Players[0]);
            // 덱에 해당 카드가 존재하는지?
            Assert.IsTrue(dataHandler.GameData.Players[0].Deck[0].SOID == expectedID);
        }

        [Test]
        public void Init_Should_SetPlayer_Properly()
        {
            Assert.IsNotNull(dataHandler.GameData.Players[1]);
            Assert.IsTrue(dataHandler.GameData.Players[1].PlayerID == 1);
        }
    }
}