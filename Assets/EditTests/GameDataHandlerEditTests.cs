using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameDataHandlerEditTests
    {
        DefaultGameDataHandler dataHandler;
        ulong firstClientID = 34;
        ulong secondClientID = 25;

        /// <summary>
        ///     실제 Scriptable Object를 Load해서 동작하도록 구현
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var resourceLoader = new DefaultResourceLoader();
            var addressableLoader = new DefaultAddressableLoader();
            var resourceManager = new ResourceManager(resourceLoader, addressableLoader);

            resourceManager.Init();

            // ILogic
            var mockLogic = new Mock<IGameLogic>();
            mockLogic
                .Setup(logic => logic.Resource)
                .Returns(resourceManager);

            var playerInfos = new List<PlayerInfo>();

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

            playerInfos.Add(secondPlayer);
            playerInfos.Add(firstPlayer);

            var mockConfig = new Mock<IGameLogicConfig>();
            var handlerFactory = new DefaultGameDataHandlerFactory(mockLogic.Object, mockConfig.Object, playerInfos);
            dataHandler = new DefaultGameDataHandler();

            dataHandler.Init(handlerFactory);
        }

        [TearDown]
        public void TearDown()
        {
            dataHandler = null;
        }

        [Test]
        public void Init_Should_Set_GameData()
        {
            Assert.IsNotNull(dataHandler.GameData);
        }

        [Test]
        public void Init_Should_SetPlayer()
        {
            Assert.IsNotNull(dataHandler.GameData.Players[firstClientID]);
            // 덱에 해당 카드가 존재하는지?
            Assert.IsTrue(dataHandler.GameData.Players[firstClientID].Deck[0].SOID == "1");
        }

        [Test]
        public void Init_Should_SetPlayer_Properly()
        {
            Assert.IsNotNull(dataHandler.GameData.Players[secondClientID]);
            Assert.IsTrue(dataHandler.GameData.Players[secondClientID].PlayerID == secondClientID);
        }
        
        [Test]
        public void DrawCardTest()
        {
            dataHandler.DrawCard(firstClientID, out var drawnCard1);
            dataHandler.DrawCard(secondClientID, out var drawnCard2);

            string expectedUID = drawnCard1.UID;

            dataHandler.TryGetCardFromHand(firstClientID, drawnCard1.UID, out var resultCard);

            Assert.AreEqual(expectedUID, resultCard.UID);
        }

        [Test]
        public void PlayCardTest()
        {
            dataHandler.DrawCard(firstClientID, out var drawnCard1);
            dataHandler.DrawCard(secondClientID, out var drawnCard2);

            string expectedUID = drawnCard1.UID;

            dataHandler.TryGetCardFromHand(firstClientID, drawnCard1.UID, out var  playedCard);
            dataHandler.RemoveCardFromHand(firstClientID, playedCard.UID);
            dataHandler.AddCardToField(firstClientID, playedCard, 0);

            var resultCard = dataHandler.GetCardFromField(firstClientID, playedCard.UID);

            Assert.IsNotNull(resultCard);
            Assert.AreEqual(expectedUID, resultCard.UID);
        }

        [Test]
        public void GetCardFrom()
        {

        }
    }
}