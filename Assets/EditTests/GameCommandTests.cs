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
            var gameEventHandler = new TriggerHandler();
            mockLogic
                .Setup(l => l.DataHandler)
                .Returns(mockDataHandler.Object);
            mockLogic
                .Setup(l => l.Trigger)
                .Returns(gameEventHandler);

            var commandFactory = new DefaultCommandFactory(mockLogic.Object);

            var playCommand = commandFactory.CreatePlayCard(32, "asdf", 0);
            var attackCommand = commandFactory.CreateAttack(32, "addd", 21, "fdsa");

            Assert.IsNotNull(playCommand);
            Assert.IsNotNull(attackCommand);
        }
    }
}