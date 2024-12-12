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
            var networkEx = new NetworkManagerEx(networkWrapper);

            MockManagers = new Mock<IManagers>();
            MockManagers
                .Setup(m => m.NetworkEx)
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
            Assert.IsNotNull(MockManagers.Object.NetworkEx);
        }

        [UnityTest]
        public IEnumerator NetworkEx_StartHost_IsHost()
        {
            yield return new WaitForSeconds(0.5f);
            MockManagers.Object.NetworkEx.StartHost();
            Assert.IsTrue(NetworkManager.Singleton.IsHost, "NetworkManager should be in host mode.");
        }

        [UnityTest]
        public IEnumerator OnConnect_Should_Invoke()
        {
            bool isInvoked = false;

            MockManagers.Object.NetworkEx.OnConnect += () =>
            {
                isInvoked = true;
            };

            yield return new WaitForSeconds(0.5f);
            MockManagers.Object.NetworkEx.StartHost();

            Assert.IsTrue(isInvoked);
        }

        [UnityTest]
        public IEnumerator Messaging_Subscribe_Send_Listen()
        {
            bool hasListen = false;

            yield return new WaitForSeconds(0.5f);
            MockManagers.Object.NetworkEx.StartHost();
            ulong id = MockManagers.Object.NetworkEx.LocalClientID;

            MockManagers.Object.NetworkEx.SubscribeMessage("HostTest", (senderID, reader) =>
            {
                if (senderID == MockManagers.Object.NetworkEx.LocalClientID)
                    hasListen = true;
            });

            Action<FastBufferWriter> messageWriter = (writer) =>
            {
                writer.WriteValueSafe("TestMessage");
                writer.WriteValueSafe(12345);
            };

            MockManagers.Object.NetworkEx.SendMessage("HostTest", id, messageWriter, NetworkDelivery.Reliable);

            Assert.IsTrue(hasListen);
        }

        [UnityTest]
        public IEnumerator Messaging_Write_Read_Correctly()
        {
            string stringValue = "";
            int intValue = 0;

            yield return new WaitForSeconds(0.5f);
            MockManagers.Object.NetworkEx.StartHost();
            ulong id = MockManagers.Object.NetworkEx.LocalClientID;

            MockManagers.Object.NetworkEx.SubscribeMessage("HostTest", (senderID, reader) =>
            {
                if (senderID == MockManagers.Object.NetworkEx.LocalClientID)
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

            MockManagers.Object.NetworkEx.SendMessage("HostTest", id, messageWriter, NetworkDelivery.Reliable);

            Assert.AreEqual("TestMessage", stringValue);
            Assert.AreEqual(12345, intValue);
        }

        [UnityTest]
        public IEnumerator CustomNetworkManager_Could_StartHost()
        {
            yield return null;

            // 일단 현재 네트워크 전부 삭제한다.

            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            }
            MockManagers = null;

            var hostNetworkGameObject = new GameObject("@HostNetwork");
            var hostNetwork = hostNetworkGameObject.AddComponent<NetworkManager>();
            var hostTransport = hostNetworkGameObject.AddComponent<UnityTransport>();
            hostTransport.ConnectionData.Port = 7778;

            hostNetwork.NetworkConfig = new NetworkConfig
            {
                NetworkTransport = hostTransport
            };

            Debug.Log(hostNetwork.NetworkConfig is null);

            hostNetwork.StartHost();

            Assert.IsTrue(hostNetwork.IsHost, "Start host is not work");

            hostNetwork.Shutdown();
            UnityEngine.Object.Destroy(hostNetwork.gameObject);

            yield break;
        }

        [UnityTest]
        public IEnumerator Two_NetworkManager()
        {
            yield return null;

            if(NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.Shutdown();
                UnityEngine.Object.Destroy(NetworkManager.Singleton.gameObject);
            }
            MockManagers = null;

            var hostNetworkObject = new GameObject("@HostNetwork");
            var hostNetwork = hostNetworkObject.AddComponent<NetworkManager>();
            var hostTransport = hostNetworkObject.AddComponent<UnityTransport>();
            hostTransport.ConnectionData.Port = 7777;

            hostNetwork.NetworkConfig = new NetworkConfig
            { NetworkTransport = hostTransport };

            var guestNetworkObject = new GameObject("@GuestNetwork");
            var guestNetwork = guestNetworkObject.AddComponent<NetworkManager>();
            var guestTransport = guestNetworkObject.AddComponent<UnityTransport>();
            guestTransport.ConnectionData.Port = 7778;

            guestNetwork.NetworkConfig = new NetworkConfig
            { NetworkTransport = guestTransport };

            hostNetwork.StartHost();
            guestNetwork.StartClient();

            Assert.IsTrue(hostNetwork.IsHost, "Start host is not work");
            Assert.IsTrue(guestNetwork.IsClient, "Start Client is not work.");

            yield break;
        }
    }
}
