using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion.Tests
{
    [TestFixture]
    public class GameLogicPlayTests
    {
        Mock<IManagers> mockManagers;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            // IManager 필요
            mockManagers = new Mock<IManagers>();

            // Load해야하기 때문에 Load Helper들도 필요
            // IResourceLoader
            var resourceLoader = new DefaultResourceLoader();

            // IAddressableLoader
            var addressableLoader = new DefaultAddressableLoader();

            // IDataManager는 ResourceManager에 대한 의존
            var resourceManager = new ResourceManager(resourceLoader, addressableLoader);

            // IManagers.Data에 대한 의존성 있음
            var dataManager = new DataManager(resourceManager);

            // IManagers 연결 설정
            mockManagers
                .Setup(m => m.Data)
                .Returns(dataManager);

            mockManagers.Object.Data.Init();
        }

        [TearDown]
        public void TearDown()
        {
            mockManagers.Reset();
        }

        // 이거 이미 테스트 있을거 같은데...
        [Test]
        public void LogicInit_ShouldMake_GameData()
        {
            // Arrange
            // IGameLogicFactory
            // PlayerInfo가 필요함
            var playerInfos = new List<PlayerInfo>();
            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            // Act
            // GameLogic 만들기
            var gameLogic = new DefaultGameLogic();
            gameLogic.Init(logicFactory);

            // Assert
            // Init까지 했으면 Data가 있어야겠지?
            Assert.IsNotNull(gameLogic.GameData);
            // Config에 따라 Player 두 자리는 생성
            Assert.IsTrue(gameLogic.GameData.Players.Length == 2);
            // 그런데 PlayerInfos가 없었기 때문에 Player는 없어야함
            Assert.IsNull(gameLogic.GameData.Players[0]);
        }

        [Test]
        public void LogicInit_ShouldMake_PlayerInfo_Properly()
        {
            // Arrange
            // IGameLogicFactory
            // PlayerInfo가 필요함
            var playerInfos = new List<PlayerInfo>();

            // TODO : 각각의 Player 데이터 등록

            var logicFactory = new DefaultGameLogicFactory(mockManagers.Object, playerInfos);

            // Act
            // GameLogic 만들기
            var gameLogic = new DefaultGameLogic();
            gameLogic.Init(logicFactory);

            // Assert
            // Init까지 했으면 Data가 있어야겠지?
            Assert.IsNotNull(gameLogic.GameData);
            // Config에 따라 Player 두 자리는 생성
            Assert.IsTrue(gameLogic.GameData.Players.Length == 2);
            // PlayerInfos가 생겼기 때문에 등록 돼야 함
            Assert.IsNotNull(gameLogic.GameData.Players[0]);
        }

        [Test]
        public void Logic_StartGame_Should_Send_Action()
        {

        }
    }
}