using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class ConcreateManagersTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (SceneManager.GetActiveScene().name != "TestScene")
            {
                yield return SceneManager.LoadSceneAsync("TestScene");
            }
        }

        [UnityTest]
        public IEnumerator Managers_Init_Should_Initialize_Instance()
        {
            // Arrange
            var mockFactory = new Mock<IManagersFactory>();

            // Mock factory methods to return placeholders
            var mockResourceLoader = new Mock<IResourceLoader>();
            var mockAddressableLoader = new Mock<IAddressableLoader>();
            var mockResourceManager = new Mock<IResourceManager>();
            var mockUIManager = new Mock<IUIManager>();
            var mockNetworkWrapper = new Mock<INetworkManagerWrapper>();
            var mockNetworkEx = new Mock<INetworkManagerEx>();
            var mockServer = new Mock<IServerManager>();
            var mockServerFactory = new Mock<IServerManagerFactory>();
            var mockClient = new Mock<IClientManager>();

            mockFactory.Setup(f => f.CreateResourceLoader()).Returns(mockResourceLoader.Object);
            mockFactory.Setup(f => f.CreateAddressableLoader()).Returns(mockAddressableLoader.Object);
            mockFactory.Setup(f => f.CreateResource(It.IsAny<IResourceLoader>(), It.IsAny<IAddressableLoader>()))
                       .Returns(mockResourceManager.Object);
            mockFactory.Setup(f => f.CreateUI(It.IsAny<IResourceManager>())).Returns(mockUIManager.Object);
            mockFactory.Setup(f => f.CreateNetworkWrapper()).Returns(mockNetworkWrapper.Object);
            mockFactory.Setup(f => f.CreateNetworkEx(It.IsAny<INetworkManagerWrapper>())).Returns(mockNetworkEx.Object);
            mockFactory.Setup(f => f.CreateServer()).Returns(mockServer.Object);
            mockFactory.Setup(f => f.CreateServerFactory(It.IsAny<IManagers>())).Returns(mockServerFactory.Object);
            mockFactory.Setup(f => f.CreateClient()).Returns(mockClient.Object);
            // Act
            Managers.Init(mockFactory.Object);

            // Assert
            Assert.IsNotNull(Managers.Instance, "Managers.Instance should be initialized.");
            Assert.IsNotNull(Managers.Instance.UI, "Managers.UI should be initialized.");
            Assert.IsNotNull(Managers.Instance.Resource, "Managers.Resource should be initialized.");
            Assert.IsNotNull(Managers.Instance.Network, "Managers.NetworkEx should be initialized.");
            Assert.IsNotNull(Managers.Instance.Server, "Managers.Server should be initialized.");
            Assert.IsNotNull(Managers.Instance.Client, "Managers.Client should be initialized.");

            yield break;
        }
    }
}