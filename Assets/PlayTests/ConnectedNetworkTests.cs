using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    public class ConnectedNetworkTests
    {
        WaitForSeconds wait = new WaitForSeconds(0.5f);

        Mock<IManagers> HostManagers;
        Mock<IManagers> GuestManagers;
        NetworkManager hostNetwork;
        NetworkManager guestNetwork;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            yield return null;

            if(hostNetwork == null)
            {
                var hostObject = new GameObject("@HostNetwork");
                hostNetwork = hostObject.AddComponent<NetworkManager>();
                var hostTransport = hostObject.AddComponent<UnityTransport>();
                hostTransport.ConnectionData.Address = "127.0.0.1";
                hostTransport.ConnectionData.Port = 7777;

                hostNetwork.NetworkConfig = new NetworkConfig
                {
                    NetworkTransport = hostTransport
                };
            }

            if(guestNetwork == null)
            {
                var guestObject = new GameObject("@GuestNetwork");
                guestNetwork = guestObject.AddComponent<NetworkManager>();
                var guestTransport = guestObject.AddComponent<UnityTransport>();
                guestTransport.ConnectionData.Address = "127.0.0.1";
                guestTransport.ConnectionData.Port = 7777;

                guestNetwork.NetworkConfig = new NetworkConfig
                {
                    NetworkTransport = guestTransport
                };
            }

            var hostFactory = new DefaultManagersFactory();
            var hostWrapper = hostFactory.CreateNetworkManagerWrapper(hostNetwork);
            var hostNetworkEx = hostFactory.CreateNetworkManager(hostWrapper);

            HostManagers = new Mock<IManagers>();
            HostManagers
                .Setup(HM => HM.Network)
                .Returns(hostNetworkEx);

            var guestFactory = new DefaultManagersFactory();
            var guestWrapper = guestFactory.CreateNetworkManagerWrapper(guestNetwork);
            var guestNetworkEx = guestFactory.CreateNetworkManager(guestWrapper);

            GuestManagers = new Mock<IManagers>();
            GuestManagers
                .Setup(GM => GM.Network)
                .Returns(guestNetworkEx);

            //HostManagers.Object.NetworkEx.StartHost();
            //GuestManagers.Object.NetworkEx.StartClient();

            yield return wait;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return null;

            if(HostManagers.Object.Network != null)
            {
                HostManagers.Object.Network.Shutdown();
                UnityEngine.Object.Destroy(hostNetwork);
            }
            HostManagers = null;

            if(GuestManagers.Object.Network != null)
            {
                GuestManagers.Object.Network.Shutdown();
                UnityEngine.Object.Destroy(guestNetwork);
            }
        }

        [UnityTest]
        public IEnumerator SetUpTest()
        {
            HostManagers.Object.Network.StartHost();
            GuestManagers.Object.Network.StartClient();

            yield return wait;

            Debug.Log(HostManagers.Object.Network.LocalID);
            Debug.Log(GuestManagers.Object.Network.LocalID);

            Assert.IsTrue(HostManagers.Object.Network.IsHost);
            Assert.IsTrue(GuestManagers.Object.Network.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator TearDownTest()
        {
            HostManagers.Object.Network.StartHost();
            GuestManagers.Object.Network.StartClient();

            yield return wait;

            Assert.IsTrue(HostManagers.Object.Network.IsHost);
            Assert.IsTrue(GuestManagers.Object.Network.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator OnConnected_Callback_Should_Invoke()
        {
            bool isConnected = false;

            HostManagers.Object.Network.OnConnect += () =>
            {
                isConnected = true;
            };

            HostManagers.Object.Network.StartHost();

            yield return wait;

            Assert.IsTrue(isConnected, "OnClientConnected callback was not invoked.");
        }

        [UnityTest]
        public IEnumerator OnClientConnected_Callback_Should_Invoke()
        {
            // Arrange
            // OnClientConnected 핸들러 등록
            bool isClientConnected = false;
            hostNetwork.OnClientConnectedCallback += (clientID) =>
            {
                Debug.Log($"Client connected with ID: {clientID}");
            };

            HostManagers.Object.Network.OnClientConnected += (clientID) =>
            {
                Debug.Log($"Client connected with ID: {clientID}");
                isClientConnected = true;
            };

            // Act
            HostManagers.Object.Network.StartHost();
            GuestManagers.Object.Network.StartClient();

            yield return wait;

            // Assert
            Assert.IsTrue(GuestManagers.Object.Network.IsClient, "Guest is not properly connected.");
            Assert.IsTrue(isClientConnected, "OnClientConnected callback was not invoked.");
        }

        /// <summary>
        ///     네트워크 연결 전에도 CustomMessaging은 등록 가능해야 합니다.
        /// </summary>
        [UnityTest]
        public IEnumerator Subscribe_Message_Before_Start_Should_Register_After_Start()
        {
            // Arrange
            HostManagers.Object.Network.StartHost();
            GuestManagers.Object.Network.StartClient();
            yield return wait;

            GuestManagers.Object.Network.SubscribeMessage("TestMessage", (clientID, reader) =>
            {
                Debug.Log($"Test Message ClientID : {clientID}");
            });
            
            var targetID = GuestManagers.Object.Network.LocalID;

            // Act
            HostManagers.Object.Network.SendMessage("TestMessage", targetID, (writer) =>
            {
                Debug.Log("Test Message");
            },
            NetworkDelivery.ReliableSequenced);

            yield return wait;

            LogAssert.Expect(LogType.Log, $"Test Message ClientID : {HostManagers.Object.Network.LocalID}");
        }

        [UnityTest]
        public IEnumerator Host_Send_Client_Should_Received()
        {
            // Arrange
            HostManagers.Object.Network.StartHost();
            GuestManagers.Object.Network.StartClient();

            yield return wait;

            bool expectedBool = false;
            string expectedString = "TestMessage";
            int expectedInt = 1234;

            ulong targetID = GuestManagers.Object.Network.LocalID;

            string receivedString = "";
            int receivedInt = 0;

            GuestManagers.Object.Network.SubscribeMessage("Host", (senderID, reader) =>
            {
                expectedBool = true;
                reader.ReadValueSafe(out receivedString);
                reader.ReadValueSafe(out receivedInt);
            });

            Action<FastBufferWriter> messageWriter = (writer) =>
            {
                writer.WriteValueSafe(expectedString);
                writer.WriteValueSafe(expectedInt);
            };

            // Act
            HostManagers.Object.Network.SendMessage("Host", targetID, messageWriter, NetworkDelivery.Reliable);

            float timeout = 1f;
            float startTime = Time.time;

            yield return new WaitUntil(() => expectedBool || (Time.time - startTime) > timeout);
            Debug.Log($"Time : {Time.time - startTime})");

            // Assert
            Assert.IsTrue(expectedBool);
            Assert.AreEqual(expectedString, receivedString);
            Assert.AreEqual(expectedInt, receivedInt);
        }
    }
}