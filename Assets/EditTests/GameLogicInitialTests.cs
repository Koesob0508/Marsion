using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameLogicInitialTests
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

        // 이거 이미 테스트 있을거 같은데...
        [Test]
        public void LogicInit_ShouldMake_GameData()
        {
            // Arrange
            // IGameLogicFactory
            // PlayerInfo가 필요함
            var playerInfos = new List<PlayerInfo>();
            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            // Act
            // GameLogic 만들기
            var gameLogic = new DefaultGameLogic();
            gameLogic.Init(logicFactory);

            // Assert
            // Init까지 했으면 Data가 있어야겠지?
            Assert.IsNotNull(gameLogic.GameData);
            // 그런데 PlayerInfos가 없었기 때문에 Player는 없어야함
            Assert.IsTrue(gameLogic.GameData.Players.Count == 0);
            Assert.IsNotNull(gameLogic.Event);
        }

        [Test]
        public void CommandFactoryTest()
        {
            var playerInfos = new List<PlayerInfo>();
            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);
            var dataHandler = new DefaultGameDataHandler();

            var gameLogic = new DefaultGameLogic();
            gameLogic.Init(logicFactory);

            var commandFactory = logicFactory.CreateCommandFactory(gameLogic);
            var attackCommand = commandFactory.CreateAttackCreature(13, "asdf", 14, "fdas");

            Assert.IsNotNull(commandFactory);
            Assert.IsNotNull(attackCommand);
        }

        [Test]
        public void LogicInit_ShouldMake_PlayerInfo_Properly()
        {
            // Arrange
            // IGameLogicFactory
            // PlayerInfo가 필요함
            var playerInfos = new List<PlayerInfo>();

            // 각각의 Player 데이터 등록
            var firstPlayer = new PlayerInfo();
            firstPlayer.ClientID = 1;
            firstPlayer.Portrait = "4";

            List<string> firstDeck = new List<string>
            {
                "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "10",
                "1", "3", "5", "7", "9",
                "2", "4", "6", "8", "10",
                "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "10"
            };
            firstPlayer.Deck = firstDeck;

            var secondPlayer = new PlayerInfo();
            secondPlayer.ClientID = 0;
            secondPlayer.Portrait = "7";

            List<string> secondDeck = new List<string>
            {
                "11", "12", "13", "14", "15",
                "16", "17", "18", "19", "20",
                "11", "13", "15", "17", "19",
                "12", "14", "16", "18", "20",
                "11", "12", "13", "14", "15",
                "16", "17", "18", "19", "20"
            };
            secondPlayer.Deck = secondDeck;

            playerInfos.Add(firstPlayer);
            playerInfos.Add(secondPlayer);

            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            // Act
            // GameLogic 만들기
            var gameLogic = new DefaultGameLogic();
            gameLogic.Init(logicFactory);

            // Assert
            // Init까지 했으면 Data가 있어야겠지?
            Assert.IsNotNull(gameLogic.GameData);
            // PlayerInfos가 생겼기 때문에 등록 돼야 함
            Assert.IsNotNull(gameLogic.GameData.Players[0]);

            bool isWrong = false;
            foreach(var card in gameLogic.GameData.Players[1].Deck)
            {
                if(int.Parse(card.SOID) > 10)
                {
                    isWrong = true;
                }
            }

            Assert.IsFalse(isWrong);
        }

        [Test]
        public void LogicInit_ShouldMake_PlayerInfo_Over2_ClientID()
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
                "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "10",
                "1", "3", "5", "7", "9",
                "2", "4", "6", "8", "10",
                "1", "2", "3", "4", "5",
                "6", "7", "8", "9", "10"
            };
            firstPlayer.Deck = firstDeck;

            var secondPlayer = new PlayerInfo();
            secondPlayer.ClientID = secondClientID;
            secondPlayer.Portrait = "7";

            List<string> secondDeck = new List<string>
            {
                "11", "12", "13", "14", "15",
                "16", "17", "18", "19", "20",
                "11", "13", "15", "17", "19",
                "12", "14", "16", "18", "20",
                "11", "12", "13", "14", "15",
                "16", "17", "18", "19", "20"
            };
            secondPlayer.Deck = secondDeck;

            playerInfos.Add(firstPlayer);
            playerInfos.Add(secondPlayer);

            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            // Act
            // GameLogic 만들기
            var gameLogic = new DefaultGameLogic();
            gameLogic.Init(logicFactory);

            // Assert
            // Init까지 했으면 Data가 있어야겠지?
            Assert.IsNotNull(gameLogic.GameData);
            // PlayerInfos가 생겼기 때문에 등록 돼야 함
            Assert.IsNotNull(gameLogic.GameData.Players[firsClientID]);
        }
    }
}