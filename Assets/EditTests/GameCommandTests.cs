using Moq;
using NUnit.Framework;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameCommandTests
    {
        [Test]
        public void Init_Test()
        {
            var mockLogic = new Mock<IGameLogic>();
            var mockDataHandler = new Mock<IGameDataHandler>();
            var gameEventHandler = new GameEventHandler();
            mockLogic
                .Setup(l => l.DataHandler)
                .Returns(mockDataHandler.Object);
            mockLogic
                .Setup(l => l.EventHandler)
                .Returns(gameEventHandler);

            var commandFactory = new DefaultGameCommandFactory(mockLogic.Object);

            var playCommand = commandFactory.CreatePlayCardCommand(32, "asdf", 0);
            var attackCommand = commandFactory.CreateAttackCommand(32, "addd", 21, "fdsa");

            Assert.IsNotNull(playCommand);
            Assert.IsNotNull(attackCommand);
        }
    }
}