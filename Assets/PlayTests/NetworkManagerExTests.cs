using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
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
            var networkEx = managersFactory.CreateNetworkManager(networkWrapper);

            MockManagers = new Mock<IManagers>();
            MockManagers
                .Setup(m => m.Network)
                .Returns(networkEx);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return null;

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                UnityEngine.Object.Destroy(networkManagerGameObject);
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
            Assert.IsNotNull(MockManagers.Object.Network);
        }

        [UnityTest]
        public IEnumerator NetworkEx_StartHost_IsHost()
        {
            yield return null;
            MockManagers.Object.Network.StartHost();
            Assert.IsTrue(NetworkManager.Singleton.IsHost, "NetworkManager should be in host mode.");
        }

        [UnityTest]
        public IEnumerator OnConnect_Should_Invoke()
        {
            bool isInvoked = false;

            MockManagers.Object.Network.OnConnect += () =>
            {
                isInvoked = true;
            };

            yield return null;
            MockManagers.Object.Network.StartHost();

            Assert.IsTrue(isInvoked);
        }

        [UnityTest]
        public IEnumerator Messaging_Subscribe_Send_Listen()
        {
            bool hasListen = false;

            yield return null;
            MockManagers.Object.Network.StartHost();
            ulong id = MockManagers.Object.Network.LocalID;

            MockManagers.Object.Network.SubscribeMessage("HostTest", (senderID, reader) =>
            {
                if (senderID == MockManagers.Object.Network.LocalID)
                    hasListen = true;
            });

            Action<FastBufferWriter> messageWriter = (writer) =>
            {
                writer.WriteValueSafe("TestMessage");
                writer.WriteValueSafe(12345);
            };

            MockManagers.Object.Network.SendMessage("HostTest", id, messageWriter, NetworkDelivery.Reliable);

            Assert.IsTrue(hasListen);
        }

        [UnityTest]
        public IEnumerator Messaging_Write_Read_Correctly()
        {
            string stringValue = "";
            int intValue = 0;

            yield return null;
            MockManagers.Object.Network.StartHost();
            ulong id = MockManagers.Object.Network.LocalID;

            MockManagers.Object.Network.SubscribeMessage("HostTest", (senderID, reader) =>
            {
                if (senderID == MockManagers.Object.Network.LocalID)
                {
                    reader.ReadValueSafe(out stringValue);
                    reader.ReadValueSafe(out intValue);
                }
            });

            Action<FastBufferWriter> messageWriter = (writer) =>
            {
                writer.WriteValueSafe("TestMessage");
                writer.WriteValueSafe(12345);
            };

            MockManagers.Object.Network.SendMessage("HostTest", id, messageWriter, NetworkDelivery.Reliable);

            Assert.AreEqual("TestMessage", stringValue);
            Assert.AreEqual(12345, intValue);
        }

        [UnityTest]
        public IEnumerator CustomNetworkManager_Could_StartHost()
        {
            yield return null;

            // 일단 현재 네트워크 전부 삭제한다.
            // ConnectedNetworkTet로 패스

            //if (NetworkManager.Singleton != null)
            //{
            //    NetworkManager.Singleton.Shutdown();
            //    UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            //}
            //MockManagers = null;

            //var hostNetworkGameObject = new GameObject("@HostNetwork");
            //var hostNetwork = hostNetworkGameObject.AddComponent<NetworkManager>();
            //var hostTransport = hostNetworkGameObject.AddComponent<UnityTransport>();
            //hostTransport.ConnectionData.Port = 7778;

            //hostNetwork.NetworkConfig = new NetworkConfig
            //{
            //    NetworkTransport = hostTransport
            //};

            //Debug.Log(hostNetwork.NetworkConfig is null);

            //hostNetwork.StartHost();

            //Assert.IsTrue(hostNetwork.IsHost, "Start host is not work");

            ////hostNetwork.Shutdown();
            ////UnityEngine.Object.Destroy(hostNetwork.gameObject);
            //hostNetworkGameObject = null;
            //hostNetwork = null;

            //if (NetworkManager.Singleton != null)
            //{
            //    NetworkManager.Singleton.Shutdown();
            //    UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            //}

            //yield break;
        }
    }
}
