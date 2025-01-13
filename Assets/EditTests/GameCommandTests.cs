using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameCommandTests
    {
        ICommandFactory commandFactory;
        ICommandHandler commandHandler;
        ITriggerHandler triggerHandler;
        /// <summary>
        ///     CommandFactory
        ///     CommandHandler
        ///     TriggerHandler
        /// </summary>
        [SetUp]
        public void SetUp()
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

            commandFactory = new DefaultCommandFactory(mockLogic.Object);
            commandHandler = new CommandHandler(mockLogic.Object);
            triggerHandler = new TriggerHandler();
        }

        [TearDown]
        public void TearDown()
        {
            commandFactory = null;
            commandHandler = null;
            triggerHandler = null;
        }

        [Test]
        public void Create_Test()
        {
            var attackCommand = commandFactory.CreateAttack(32, "addd", 21, "fdsa");
            var spawnCommand = commandFactory.CreatePlayCreature(32, "asdf", 0);

            Assert.IsNotNull(spawnCommand);
            Assert.IsNotNull(attackCommand);
        }

        [Test]
        public void BaseCommand_Test()
        {
            //var mockLogic = new Mock<IGameLogic>();
            //mockLogic
            //    .Setup(l => l.Trigger)
            //    .Returns(triggerHandler);
            //var baseCommand = new BaseCommand(mockLogic.Object, null, TriggerType.Spawn);

            //commandHandler.Add(baseCommand);

            Assert.Pass();
        }

        [Test]
        public void PlayCreature_Test()
        {
            var mockLogic = new Mock<IGameLogic>();
            mockLogic
                .Setup(l => l.Trigger)
                .Returns(triggerHandler);
            var mockCommand = new Mock<ICommand>();
            mockCommand
                .Setup(c => c.Execute())
                .Callback(() =>
                {
                    triggerHandler.Trigger(TriggerType.PlayCreature);
                });

            triggerHandler.Register(TriggerType.PlayCreature, (data) =>
            {
                Debug.Log("Play Creature");
            });

            commandHandler.Add(mockCommand.Object);

            LogAssert.Expect(LogType.Log, "Play Creature");
        }
    }
}