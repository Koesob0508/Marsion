using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class ManagersTests
    {
        [Test]
        public void Managers_Init()
        {
            // Arrange
            var mockFactory = new Mock<IManagerFactory>();
            var mockUI = new Mock<UIManager>();
            var mockLoader = new Mock<IResourceLoader>();
            var mockResource = new Mock<IResourceManager>();
            var mockCard = new Mock<CardManager>();
            var mockData = new Mock<DataManager>();
            var mockNetwork = new Mock<MarsNetwork>();
            var mockServer = new Mock<ServerManager>();
            var mockClient = new Mock<ClientManager>();

            mockFactory.Setup(f => f.CreateResourceLoader()).Returns(mockLoader.Object);
            mockFactory.Setup(f => f.CreateResource(mockLoader.Object)).Returns(mockResource.Object);
            mockFactory.Setup(f => f.CreateUI(mockResource.Object)).Returns(mockUI.Object);
            mockFactory.Setup(f => f.CreateCard()).Returns(mockCard.Object);
            mockFactory.Setup(f => f.CreateData()).Returns(mockData.Object);
            mockFactory.Setup(f => f.CreateNetwork()).Returns(mockNetwork.Object);
            mockFactory.Setup(f => f.CreateServer()).Returns(mockServer.Object);
            mockFactory.Setup(f => f.CreateClient()).Returns(mockClient.Object);

            // Act
            Managers.Init(mockFactory.Object);

            // Assert
            Assert.IsNotNull(Managers.Instance);
            Assert.AreEqual(mockUI.Object, Managers.Instance.UI);
            Assert.AreEqual(mockResource.Object, Managers.Instance.Resource);
            Assert.AreEqual(mockCard.Object, Managers.Instance.Card);
            Assert.AreEqual(mockData.Object, Managers.Instance.Data);
            Assert.AreEqual(mockNetwork.Object, Managers.Instance.Network);
            Assert.AreEqual(mockServer.Object, Managers.Instance.Server);
            Assert.AreEqual(mockClient.Object, Managers.Instance.Client);
        }

        [Test]
        public void Managers_Clear()
        {
            // Arrange
            var mockUI = new Mock<UIManager>();
            var mockFactory = new Mock<IManagerFactory>();
            var mockResourceLoader = new Mock<IResourceLoader>();
            var mockResourceManager = new Mock<IResourceManager>();
            var mockUIManager = new Mock<UIManager>(mockResourceManager.Object);

            mockFactory.Setup(f => f.CreateResourceLoader()).Returns(mockResourceLoader.Object);
            mockFactory.Setup(f => f.CreateResource(mockResourceLoader.Object)).Returns(mockResourceManager.Object);
            mockFactory.Setup(f => f.CreateUI(mockResourceManager.Object)).Returns(mockUI.Object);

            Managers.Init(mockFactory.Object);

            // Act
            Managers.Clear();

            // Assert
            mockUI.Verify(ui => ui.Clear(), Times.Once);
        }
    }
}