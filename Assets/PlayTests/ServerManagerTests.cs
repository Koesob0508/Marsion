using UnityEngine;
using Marsion;
using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    /// <summary>
    ///     사실 지금 당장은 ServerManager Test 필요성이 없다.
    ///     일단 GPT 의도대로 둔다.
    /// </summary>
    [TestFixture]
    public class ServerManagerTests
    {
        //private Mock<INetworkManagerEx> _mockNetworkManager;
        //private Mock<IServerFactory> _mockServerFactory;
        //private Mock<DraftServer> _mockDraftServer;
        //private Mock<GameServerEx> _mockGameServerEx;
        //private GameObject _serverManagerObject;
        //private ServerManager _serverManager;

        //[UnitySetUp]
        //public IEnumerator SetUp()
        //{
        //    // Arrange
        //    _mockNetworkManager = new Mock<INetworkManagerEx>();
        //    _mockServerFactory = new Mock<IServerFactory>();
        //    _mockDraftServer = new Mock<DraftServer>();
        //    _mockGameServerEx = new Mock<GameServerEx>();

        //    _mockServerFactory.Setup(f => f.CreateDraftServer()).Returns(_mockDraftServer.Object);
        //    _mockServerFactory.Setup(f => f.CreateGameServerEx()).Returns(_mockGameServerEx.Object);

        //    _serverManagerObject = new GameObject("@ServerManager");
        //    _serverManager = _serverManagerObject.AddComponent<ServerManager>();

        //    yield return null;
        //}

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    Object.Destroy(_serverManagerObject);
        //    yield return null;
        //}

        //[UnityTest]
        //public IEnumerator Init_Should_Initialize_DraftServer_And_GameServerEx()
        //{
        //    // Act
        //    _serverManager.Init(_mockNetworkManager.Object, _mockServerFactory.Object);

        //    // Assert
        //    _mockServerFactory.Verify(f => f.CreateDraftServer(), Times.Once);
        //    _mockDraftServer.Verify(ds => ds.Init(), Times.Once);

        //    _mockServerFactory.Verify(f => f.CreateGameServerEx(), Times.Once);
        //    _mockGameServerEx.Verify(gs => gs.Init(), Times.Once);

        //    yield break;
        //}

        //[UnityTest]
        //public IEnumerator OnConnect_Should_Initialize_And_Register_Client()
        //{
        //    // Arrange
        //    _mockNetworkManager.Setup(nm => nm.IsHost).Returns(true);
        //    _mockNetworkManager.Setup(nm => nm.LocalID).Returns(1234UL);

        //    _serverManager.Init(_mockNetworkManager.Object, _mockServerFactory.Object);

        //    // Act
        //    var onConnectMethod = typeof(ServerManager).GetMethod("OnConnect",
        //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //    onConnectMethod.Invoke(_serverManager, null);

        //    // Assert
        //    _mockDraftServer.Verify(ds => ds.Init(), Times.Once);
        //    _mockGameServerEx.Verify(gs => gs.Init(), Times.Once);
        //    _mockDraftServer.Verify(ds => ds.AddState(1234UL), Times.Once);

        //    yield break;
        //}

        //[UnityTest]
        //public IEnumerator OnClientJoin_Should_Register_Client()
        //{
        //    // Arrange
        //    _mockNetworkManager.Setup(nm => nm.ServerID).Returns(1000UL);

        //    _serverManager.Init(_mockNetworkManager.Object, _mockServerFactory.Object);

        //    // Act
        //    var onClientJoinMethod = typeof(ServerManager).GetMethod("OnClientJoin",
        //        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //    onClientJoinMethod.Invoke(_serverManager, new object[] { 2000UL });

        //    // Assert
        //    _mockDraftServer.Verify(ds => ds.AddState(2000UL), Times.Once);
        //    yield break;
        //}
    }
}
