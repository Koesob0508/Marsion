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
        IEventHandler triggerHandler;
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
            var gameEventHandler = new EventHandler();
            mockLogic
                .Setup(l => l.DataHandler)
                .Returns(mockDataHandler.Object);
            mockLogic
                .Setup(l => l.Event)
                .Returns(gameEventHandler);

            commandFactory = new DefaultCommandFactory(mockLogic.Object);
            commandHandler = new CommandHandler(mockLogic.Object);
            triggerHandler = new EventHandler();
        }

        [TearDown]
        public void TearDown()
        {
            commandFactory = null;
            commandHandler = null;
            triggerHandler = null;
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
                .Setup(l => l.Event)
                .Returns(triggerHandler);
            var mockCommand = new Mock<ICommand>();
            mockCommand
                .Setup(c => c.Execute())
                .Callback(() =>
                {
                    triggerHandler.Trigger(EventType.PlayCard);
                });

            triggerHandler.Register(EventType.PlayCard, (data) =>
            {
                Debug.Log("Play Creature");
            });

            commandHandler.Add(mockCommand.Object);

            LogAssert.Expect(LogType.Log, "Play Creature");
        }
    }
}