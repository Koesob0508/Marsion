using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameLogicCommandTests
    {
        readonly ulong firstClientID = 17;
        readonly ulong secondClientID = 23;
        IGameLogic Logic;

        [SetUp]
        public void SetUp()
        {
            var mockManagers = new Mock<IManagers>();
            var resourceLoader = new DefaultResourceLoader();
            var addressableLoader = new DefaultAddressableLoader();
            var resourceManager = new ResourceManager(resourceLoader, addressableLoader);

            mockManagers
                .Setup(m => m.Resource)
                .Returns(resourceManager);

            mockManagers.Object.Resource.Init();

            var playerInfos = new List<PlayerInfo>();

            // 각각의 Player 데이터 등록
            var firstPlayer = new PlayerInfo();
            firstPlayer.ClientID = firstClientID;
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

            Logic = new DefaultGameLogic();
            Logic.Init(logicFactory);
        }

        [TearDown]
        public void TearDown()
        {
            Logic = null;
        }

        [Test]
        public void StartTurnTest()
        {
            bool isStarted = false;
            int countOfDraw = 0;

            Logic.Event.Register(EventType.StartGame, (eventData) =>
            {
                isStarted = true;
            });

            Logic.Event.Register(EventType.DrawCard, (eventData) =>
            {
                countOfDraw++;
            });

            Logic.StartGame();

            Assert.IsTrue(Logic.GameData.CurrentPlayer.Hand.Count == 4, $"Current player's count of hand is {Logic.GameData.CurrentPlayer.Hand.Count}");
            Logic.DataHandler.TryGetPlayer(Logic.DataHandler.GetOpponentPlayerID(Logic.GameData.CurrentPlayer.PlayerID), out var opponentPlayer);
            Assert.IsTrue(opponentPlayer.Hand.Count == 4, $"Current opponent's count of hand is {opponentPlayer.Hand.Count}");
            Assert.IsTrue(isStarted, "Event not triggered.");
            Assert.IsTrue(countOfDraw == 3, $"Game Start action must call draw card 3 times. Current {countOfDraw} ");
        }

        [Test]
        public void EndTurnTest()
        {
            bool isEnded = false;
            int countOfTurnStart = 0;

            Logic.Event.Register(EventType.EndTurn, (_) =>
            {
                isEnded = true;
            });

            Logic.Event.Register(EventType.StartTurn, (eventData) =>
            {
                countOfTurnStart++;
            });

            var opponentID = Logic.DataHandler.GetOpponentPlayerID(Logic.DataHandler.CurrentPlayer.PlayerID);

            Logic.StartGame();
            Logic.EndTurn();

            Assert.IsTrue(Logic.DataHandler.CurrentPlayer.PlayerID == opponentID, "턴 안 바뀜");
            Assert.IsTrue(isEnded, "End event not triggered.");
            Assert.IsTrue(countOfTurnStart == 2, $"Turn start action must call 2 times. Current {countOfTurnStart}");
        }

        [Test]
        public void PlayCardTest()
        {
            bool isCasted = false;

            Logic.Event.Register(EventType.CastSpell, _ =>
            {
                isCasted = true;
            });

            // Action
            Logic.StartGame();

            var playerID = Logic.DataHandler.CurrentPlayer.PlayerID;
            Logic.DataHandler.TryGetPlayer(playerID, out var player);
            var cardUID = player.Hand[0].UID;

            Logic.TryPlayCard(playerID, cardUID, 0);

            Assert.IsTrue(player.MaxMana == 1, $"Current max mana : {player.MaxMana}");
            Assert.IsTrue(player.Mana == 0, $"Current mana {player.Mana}");
            Assert.IsTrue(player.Field.Count != 0, $"Current field count : {player.Field.Count}");
            Assert.IsNotNull(Logic.DataHandler.GetCardFromField(playerID, cardUID), "없음");

            var cardUID2 = player.Hand[0].UID;
            Logic.TryPlayCard(playerID, cardUID2, 1);

            Assert.IsTrue(player.Field.Count == 1, $"Current field count : {player.Field.Count}");
            Assert.IsTrue(Logic.DataHandler.TryGetCardFromHand(playerID, cardUID2, out var card), $"{card.UID} did not exists.");

            Assert.IsTrue(isCasted, "전투의 함성이 호출되지 않음. 단 전투의 함성을 안 꼈을 수 있음 확인 필요");
            Assert.IsTrue(player.Hand.Count == 4, $"Current hand count : {player.Hand.Count}");
        }
    }
}