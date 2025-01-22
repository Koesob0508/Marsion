using Marsion.UI;
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
            Logic.DataHandler.TryGetPlayer(Logic.DataHandler.GetOpponentPlayerID(Logic.GameData.CurrentPlayer.ID), out var opponentPlayer);
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

            var opponentID = Logic.DataHandler.GetOpponentPlayerID(Logic.DataHandler.CurrentPlayer.ID);

            Logic.StartGame();
            Logic.EndTurn();

            Assert.IsTrue(Logic.DataHandler.CurrentPlayer.ID == opponentID, "턴 안 바뀜");
            Assert.IsTrue(isEnded, "End event not triggered.");
            Assert.IsTrue(countOfTurnStart == 2, $"Turn start action must call 2 times. Current {countOfTurnStart}");
        }

        [Test]
        public void PlayCardTest()
        {
            bool isCasted = false;

            Logic.Event.Register(EventType.CompositionSpell, _ =>
            {
                isCasted = true;
            });

            // Action
            Logic.StartGame();

            var playerID = Logic.DataHandler.CurrentPlayer.ID;
            Logic.DataHandler.TryGetPlayer(playerID, out var player);
            var cardUID = player.Hand[0].UID;

            Logic.TryPlayCard(playerID, cardUID, 0);

            Assert.IsTrue(player.MaxMana == 1, $"Current max mana : {player.MaxMana}");
            Assert.IsTrue(player.Mana == 0, $"Current mana {player.Mana}");
            Assert.IsTrue(player.Field.Count != 0, $"Current field count : {player.Field.Count}");
            Assert.IsNotNull(Logic.DataHandler.TryGetCardFromField(playerID, cardUID, out var fieldCard), "없음");

            var cardUID2 = player.Hand[0].UID;
            Logic.TryPlayCard(playerID, cardUID2, 1);

            Assert.IsTrue(player.Field.Count == 1, $"Current field count : {player.Field.Count}");
            Assert.IsTrue(Logic.DataHandler.TryGetCardFromHand(playerID, cardUID2, out var card), $"{card.UID} did not exists.");

            Assert.IsTrue(isCasted, "전투의 함성이 호출되지 않음. 단 전투의 함성을 안 꼈을 수 있음 확인 필요");
            Assert.IsTrue(player.Hand.Count == 4, $"Current hand count : {player.Hand.Count}");
        }

        [Test]
        public void DeompositionAbilityTest()
        {
            // Arrange
            bool isSpawnTriggered = false;
            bool isDeompositionTriggered = false;

            Logic.StartGame();
            var currentPlayerID = Logic.DataHandler.CurrentPlayer.ID;
            var opponentPlayerID = Logic.DataHandler.GetOpponentPlayerID(currentPlayerID);
            
            Logic.DataHandler.TryGetPlayer(currentPlayerID, out var player);
            var cardUID = player.Hand[0].UID;

            Logic.Event.Register(EventType.SpawnCreatureFromHand, _ =>
            {
                isSpawnTriggered = true;
            });

            Logic.Event.Register(EventType.AfterSpawn, _ =>
            {
                Logic.CommandHandler.Add(Logic.CommandFactory.CreateKillCreature(opponentPlayerID, null, currentPlayerID, cardUID));
            });

            Logic.Event.Register(EventType.DecompositionSpell, _ =>
            {
                isDeompositionTriggered = true;
            });

            // Action
            Logic.TryPlayCard(currentPlayerID, cardUID, 0);

            // Assert
            Assert.IsTrue(isSpawnTriggered, "Spawn event not triggered");
            Assert.IsTrue(isDeompositionTriggered, "Decomposition event not triggered");
            Assert.IsTrue(Logic.DataHandler.CurrentPlayer.Field.Count == 0);
        }

        [Test]
        public void TriggerAbilityTest()
        {
            /// [시나리오]
            /// 모든 생물들은 트리거 : 상대가 소환했을 때, 그 생물을 처치한다. 효과를 갖고 있다.
            /// 플레이어 1이 생물 A을 소환한다. 트리거가 등록된다.
            /// 플레이어 2가 생물 B을 소환한다.
            /// 생물 A의 트리거가 동작한다. 생물 B를 처치한다.
            /// [결과]
            /// 플레이어 2의 필드에는 아무것도 없어야한다.
            /// Kill Event가 호출됐어야 한다.

            // Arrange
            bool isKillTriggered = false;
            Logic.Event.Register(EventType.Kill, _ =>
            {
                isKillTriggered = true;
            });

            Logic.StartGame();
            var firstPlayerID = Logic.DataHandler.CurrentPlayer.ID;
            var secondPlayerID = Logic.DataHandler.GetOpponentPlayerID(firstPlayerID);

            Logic.DataHandler.TryGetPlayer(firstPlayerID, out var player);
            var firstCardUID = player.Hand[0].UID;

            // Action
            Logic.TryPlayCard(firstPlayerID, firstCardUID, 0);

            Logic.EndTurn();

            var secondCardUID = Logic.DataHandler.CurrentPlayer.Hand[0].UID;
            Logic.TryPlayCard(secondPlayerID, secondCardUID, 0);

            // Assert
            /// Die의 경우, 두 가지 케이스가 있을 수 있음
            /// 체력이 0이 된 경우,
            /// Kill Commad에 의해 체력과 무관하게 죽은 경우
            /// 전자는 Die Trigger만 호출
            /// 후자는 Kill Trigger 이후, Die Trigger 호출
            Assert.IsTrue(isKillTriggered, "Kill Command가 없었음");
            Assert.IsTrue(Logic.DataHandler.CurrentPlayer.Field.Count == 0);
        }
    }
}