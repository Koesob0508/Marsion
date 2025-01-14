using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameCommandTriggerTests
    {
        Mock<IManagers> mockManagers;
        readonly ulong firsClientID = 31;
        readonly ulong secondClientID = 72;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            // IManager 필요
            mockManagers = new Mock<IManagers>();

            // Load해야하기 때문에 Load Helper들도 필요
            // IResourceLoader
            var resourceLoader = new DefaultResourceLoader();

            // IAddressableLoader
            var addressableLoader = new DefaultAddressableLoader();

            // IDataManager는 ResourceManager에 대한 의존
            var resourceManager = new ResourceManager(resourceLoader, addressableLoader);

            // IManagers 연결 설정
            mockManagers
                .Setup(m => m.Resource)
                .Returns(resourceManager);

            mockManagers.Object.Resource.Init();
        }

        [TearDown]
        public void TearDown()
        {
            mockManagers.Reset();
        }

        [Test]
        public void Spawn_Test()
        {
            // Arrange
            // IGameLogicFactory
            // PlayerInfo가 필요함
            var playerInfos = new List<PlayerInfo>();

            // TODO : 각각의 Player 데이터 등록
            var firstPlayer = new PlayerInfo();
            firstPlayer.ClientID = firsClientID;
            firstPlayer.Portrait = "4";

            List<string> firstDeck = new List<string>
            {
                "999", "999", "999", "999", "999",
                "999", "999", "999", "999", "999",
                "999", "999", "999", "999", "999",
                "999", "999", "999", "999", "999"
            };
            firstPlayer.Deck = firstDeck;

            var secondPlayer = new PlayerInfo();
            secondPlayer.ClientID = secondClientID;
            secondPlayer.Portrait = "7";

            List<string> secondDeck = new List<string>
            {
                "999", "999", "999", "999", "999",
                "999", "999", "999", "999", "999",
                "999", "999", "999", "999", "999",
                "999", "999", "999", "999", "999"
            };
            secondPlayer.Deck = secondDeck;

            playerInfos.Add(firstPlayer);
            playerInfos.Add(secondPlayer);

            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            // Act
            // GameLogic 만들기
            var gameLogic = new DefaultGameLogic();
            gameLogic.Init(logicFactory);
            gameLogic.StartGame();

            // Assert
            // Init까지 했으면 Data가 있어야겠지?
            Assert.IsNotNull(gameLogic.GameData);
            // PlayerInfos가 생겼기 때문에 등록 돼야 함
            Assert.IsNotNull(gameLogic.GameData.Players[firsClientID]);

            var currentPlayer = gameLogic.GameData.CurrentPlayer;
            var targetCardUID = currentPlayer.Hand[0].UID;

            Assert.IsTrue(currentPlayer.Hand.Count == 4, $"");

            var playCreatureCommand = gameLogic.CommandFactory.CreatePlayCreature(currentPlayer.PlayerID, targetCardUID, 0);
            gameLogic.CommandHandler.Add(playCreatureCommand);

            Assert.IsNotNull(gameLogic.DataHandler.GetCardFromField(currentPlayer.PlayerID, targetCardUID));
            Assert.IsTrue(currentPlayer.Hand.Count == 4, $"{currentPlayer.Hand.Count}");
        }
    }
}