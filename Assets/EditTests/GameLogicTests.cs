using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Marsion.Tests
{
    [TestFixture]
    public class DefaultGameLogicTest
    {
        [Test]
        public void DefaultGameLogic_Init_Should_Generate_GameData()
        {
            var mockManagers = new Mock<IManagers>();
            var mockDataManager = new Mock<IDataManager>();

            mockManagers
                .Setup(m => m.Data)
                .Returns(mockDataManager.Object);

            var gameLogic = new DefaultGameLogic();

            var playerInfos = new List<PlayerInfo>();
            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            gameLogic.Init(logicFactory);

            Assert.IsNotNull(gameLogic.GameData);
        }
    }
}