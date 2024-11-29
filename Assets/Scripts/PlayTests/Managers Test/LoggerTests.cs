using NUnit.Framework;
using Moq;

namespace Marsion.Tests
{
    [TestFixture]
    public class LoggerTests
    {
        private Mock<ILogger> mockLogger;

        [SetUp]
        public void Setup()
        {
            mockLogger = new Mock<ILogger>();

            Logger.SetLogger(mockLogger.Object);
        }

        [Test]
        public void Log_Should_CallLoggerWithCorrectMessage()
        {
            string expectedMessage = "[DataManager] : <color=black><b>Test log message</b></color>";

            Logger.Log<DataManager>("Test log message", colorName: "black");

            mockLogger.Verify(logger => logger.Log(expectedMessage), Times.Once);
        }
    }
}