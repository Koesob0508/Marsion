using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class ResourceManagerTests
    {
        [Test]
        public void Load_ShouldCallResourceLoader()
        {
            // Arrange
            var mockLoader = new Mock<IResourceLoader>();
            mockLoader.Setup(l => l.Load<GameObject>("TestPath")).Returns(new GameObject());
            var resourceManager = new ResourceManager(mockLoader.Object);

            // Act
            var result = resourceManager.Load<GameObject>("TestPaht");

            // Assert
            Assert.IsNotNull(result);
            mockLoader.Verify(l => l.Load<GameObject>("TestPath"), Times.Once);
        }

        [Test]
        public void Instantiate_ShouldReturnInstantiatedObject()
        {
            // Arrange
            var mockLoader = new Mock<IResourceLoader>();
            var prefab = new GameObject("Prefab");
            mockLoader.Setup(l => l.Load<GameObject>("TestPath")).Returns(prefab);
            var resourceManager = new ResourceManager(mockLoader.Object);

            // Act
            var instance = resourceManager.Instantiate("TestPath");

            // Assert
            Assert.IsNotNull(instance);
            Assert.AreEqual("Prefab", instance.name); // "(Clone)" 제거 확인
        }

        [Test]
        public void Destroy_ShouldDestroyObject()
        {
            // Arrange
            var mockLoader = new Mock<IResourceLoader>();
            var resourceManager = new ResourceManager(mockLoader.Object);
            var go = new GameObject("ToDestroy");

            // Act
            resourceManager.Destroy(go);

            // Assert
            Assert.IsTrue(go == null || go.Equals(null));
        }

        [Test]
        public void Instantiate_ShouldLogErrorWhenPrefabNotFound()
        {
            // Arrange
            var mockLoader = new Mock<IResourceLoader>();
            mockLoader.Setup(l => l.Load<GameObject>("InvalidPath")).Returns((GameObject)null);
            var resourceManager = new ResourceManager(mockLoader.Object);

            // Act
            LogAssert.Expect(LogType.Warning, "Failed to load prefab : InvalidPath");
            var result = resourceManager.Instantiate("InvalidPath");

            // Assert
            Assert.IsNull(result);
        }
    }
}