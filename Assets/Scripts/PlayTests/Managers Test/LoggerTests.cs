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

        [Test]
        public void LogWarning_Should_CallLoggerWithCorrectMessage()
        {
            // 테스트할 메시지
            string expectedMessage = "[DataManager] : <color=yellow><b>Test warning message</b></color>";

            // 실제 로깅 메서드 호출
            Logger.LogWarning<DataManager>("Test warning message", "yellow");

            // 모킹된 logger의 LogWarning 메서드가 호출되었는지 검증
            mockLogger.Verify(logger => logger.LogWarning(expectedMessage), Times.Once);
        }

        [Test]
        public void LogError_Should_CallLoggerWithCorrectMessage()
        {
            // 테스트할 메시지
            string expectedMessage = "[Player] : <color=red><b>Test error message</b></color>";

            // 실제 로깅 메서드 호출
            Logger.LogError<Player>("Test error message", "red");

            // 모킹된 logger의 LogError 메서드가 호출되었는지 검증
            mockLogger.Verify(logger => logger.LogError(expectedMessage), Times.Once);
        }
    }
}