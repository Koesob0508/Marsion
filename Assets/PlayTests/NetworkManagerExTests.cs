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
        private Mock<IManagers> MockManagers;
        private GameObject networkManagerGameObject;

        [UnitySetUp]
        public IEnumerator GlobalSetUp()
        {
            if (SceneManager.GetActiveScene().name != "TestScene")
            {
                yield return SceneManager.LoadSceneAsync("TestScene");
            }
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return null;

            if (NetworkManager.Singleton == null)
            {
                networkManagerGameObject = new GameObject("@NetworkManager");
                networkManagerGameObject.AddComponent<NetworkManager>();
            }

            var managersFactory = new DefaultManagersFactory();
            var networkWrapper = managersFactory.CreateNetworkManagerWrapper();
            var networkEx = new NetworkManagerEx(networkWrapper);

            MockManagers = new Mock<IManagers>();
            MockManagers
                .Setup(m => m.NetworkEx)
                .Returns(networkEx);
        }

        [UnityTearDown]
        public void TearDown()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                Object.Destroy(networkManagerGameObject);
            }

            MockManagers = null;
        }

        [UnityTest]
        public IEnumerator AfterSceneLoaded_NetworkManager_Should_BeNotNull()
        {
            var networkManager = GameObject.Find("@NetworkManager")?.GetComponent<NetworkManager>();
            Assert.IsNotNull(networkManager, "NetworkManager should not be null after the scene is loaded.");
            yield break;
        }

        [UnityTest]
        public IEnumerator NetworkManager_Should_BeNotNull()
        {
            yield return null;
            Assert.IsNotNull(MockManagers.Object.NetworkEx);
        }

        [UnityTest]
        public IEnumerator NetworkEx_StartHost_IsHost()
        {
            yield return null;
            MockManagers.Object.NetworkEx.StartHost();
            Assert.IsTrue(NetworkManager.Singleton.IsHost, "NetworkManager should be in host mode.");
        }
    }
}
