using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameLogicPlayTests
    {
        Mock<IManagers> mockManagers;
        ulong firsClientID = 31;
        ulong secondClientID = 72;

        DefaultGameLogic logic;

        /// <summary>
        ///     관련 내용은 EditTest에 GameLogicTest 참고
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            // Arrange
            mockManagers = new Mock<IManagers>();
            var resourceLoader = new DefaultResourceLoader();

            // IAddressableLoader
            var addressableLoader = new DefaultAddressableLoader();

            // IDataManager는 ResourceManager에 대한 의존
            var resourceManager = new ResourceManager(resourceLoader, addressableLoader);

            // IManagers.Data에 대한 의존성 있음
            var dataManager = new DataManager(resourceManager);

            // IManagers 연결 설정
            mockManagers
                .Setup(m => m.Data)
                .Returns(dataManager);

            mockManagers.Object.Data.Init();

            // IGameLogicFactory
            // PlayerInfo가 필요함
            var playerInfos = new List<PlayerInfo>();

            // 각각의 Player 데이터 등록
            var firstPlayer = new PlayerInfo();
            firstPlayer.ClientID = firsClientID;
            firstPlayer.Portrait = "4";

            List<string> firstDeck = new List<string>
            {
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1"
            };
            firstPlayer.Deck = firstDeck;

            var secondPlayer = new PlayerInfo();
            secondPlayer.ClientID = secondClientID;
            secondPlayer.Portrait = "7";

            List<string> secondDeck = new List<string>
            {
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1",
                "1", "1", "1", "1", "1"
            };
            secondPlayer.Deck = secondDeck;

            playerInfos.Add(firstPlayer);
            playerInfos.Add(secondPlayer);

            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            // Act
            // GameLogic 만들기
            logic = new DefaultGameLogic();
            logic.Init(logicFactory);
        }

        [TearDown]
        public void TearDown()
        {
            mockManagers.Reset();
            logic = null;
        }

        [Test]
        public void LogicInit_ShouldMake_PlayerInfo_Over2_ClientID()
        {
            // Assert
            // Init까지 했으면 Data가 있어야겠지?
            Assert.IsNotNull(logic.GameData);
            // PlayerInfos가 생겼기 때문에 등록 돼야 함
            Assert.IsNotNull(logic.GameData.Players[firsClientID]);
        }

        [Test]
        public void Logic_StartGame_Should_Send_Action()
        {
            logic.SendDataUpdated += (gameData) =>
            {
                Debug.Log("UpdateData");
            };

            logic.SendGameStarted += () =>
            {
                Debug.Log("StartGame");
            };

            logic.SendTurnStarted += () =>
            {
                Debug.Log("StartTurn");
            };

            logic.StartGame();

            LogAssert.Expect(LogType.Log, "UpdateData");
            LogAssert.Expect(LogType.Log, "StartGame");
            LogAssert.Expect(LogType.Log, "StartTurn");
        }

        [Test]
        public void DrawCard_Test()
        {
            ulong currentPlayerID = 0;
            string playCardUID = "";
            // OnDataUpdated로 GameData랑 비교해야함.
            logic.SendDataUpdated += (data) =>
            {
                currentPlayerID = data.CurrentPlayer.PlayerID;
                playCardUID = data.GetPlayer(data.CurrentPlayer.PlayerID).Hand[0].UID;
            };

            logic.SendCardDrawn += (playerID, cardUID) =>
            {
                Debug.Log($"Player ID : {playerID}");
                Debug.Log($"Card UID : {cardUID}");
            };

            logic.StartGame();
            // StartTurn까지는 자동으로 이뤄짐.
            logic.TrySpawnCard(currentPlayerID, playCardUID, 0);

            Assert.Pass();
        }

        [Test]
        public void TrySpawn_Test()
        {
            // Arrange
            ulong targetPlayerID = 0;
            string targetCardUID = "";

            bool isSuccessed = false;
            ulong playPlayerID = 0;
            string playCardUID = "";

            bool isFirstUpdate = true;

            //logic.Trigger.RegisterEvent("UpdateData", (data) =>
            //{
            //    if (isFirstUpdate)
            //    {
            //        targetPlayerID = logic.DataHandler.CurrentPlayer.PlayerID;
            //        targetCardUID = logic.DataHandler.GetPlayer(targetPlayerID).Hand[0].UID;

            //        isFirstUpdate = false;
            //    }
            //});

            //logic.Trigger.RegisterEvent("PlayCard", (data) =>
            //{
            //    isSuccessed = data.Succeeded;
            //    playPlayerID = data.PlayerID;
            //    playCardUID = data.CardUID;
            //});

            logic.StartGame();

            // Act
            logic.TrySpawnCard(targetPlayerID, targetCardUID, 0);

            // Assert
            Assert.IsTrue(isSuccessed);
            Assert.AreEqual(targetPlayerID, playPlayerID);
            Assert.AreEqual(targetCardUID, playCardUID);
        }
    }
}