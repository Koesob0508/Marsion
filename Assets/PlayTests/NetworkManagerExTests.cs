using Moq;
using NUnit.Framework;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class NetworkManagerExTests
    {
        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return SceneManager.LoadSceneAsync("TestScene");
        }

        // NetworkManager 감지
        [UnityTest]
        public IEnumerator AfterSceneLoaded_NetworkManager_Should_BeNotNull()
        {
            var networkManager = GameObject.Find("@NetworkManager").GetComponent<NetworkManager>();
            // Assert: Verify the scene is correctly loaded
            Assert.IsNotNull(networkManager);

            yield break;
        }

        // 어차피 factory에서 NetworkManager 찾는 작업을 한다.
        [UnityTest]
        public IEnumerator NetworkManager_Should_BeNotNull()
        {
            yield return null;

            var managersFactory = new DefaultManagersFactory();
            var networkWrapper = managersFactory.CreateNetworkManagerWrapper();
            var networkEx = new NetworkManagerEx(networkWrapper);
            var mockManagers = new Mock<IManagers>();

            mockManagers
                .Setup(m => m.NetworkEx)
                .Returns(networkEx);

            Assert.IsNotNull(mockManagers.Object.NetworkEx);
        }

        // Managers.NetworkEx에서 StartHost, IsHost 확인
        [UnityTest]
        public IEnumerator NetworkEx_StartHost_IsHost()
        {
            yield return null;

            // Arrange
            var managersFactory = new DefaultManagersFactory();
            var networkWrapper = managersFactory.CreateNetworkManagerWrapper();
            var networkEx = new NetworkManagerEx(networkWrapper);
            var mockManagers = new Mock<IManagers>();

            mockManagers
                .Setup(m => m.NetworkEx)
                .Returns(networkEx);

            // Act
            mockManagers.Object.NetworkEx.StartHost();

            // Assert
            Assert.IsTrue(NetworkManager.Singleton.IsHost);
        }
    }
}