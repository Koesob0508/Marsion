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
            var hostNetworkEx = hostFactory.CreateNetworkManagerEx(hostWrapper);

            HostManagers = new Mock<IManagers>();
            HostManagers
                .Setup(HM => HM.NetworkEx)
                .Returns(hostNetworkEx);

            var guestFactory = new DefaultManagersFactory();
            var guestWrapper = guestFactory.CreateNetworkManagerWrapper(guestNetwork);
            var guestNetworkEx = guestFactory.CreateNetworkManagerEx(guestWrapper);

            GuestManagers = new Mock<IManagers>();
            GuestManagers
                .Setup(GM => GM.NetworkEx)
                .Returns(guestNetworkEx);

            //HostManagers.Object.NetworkEx.StartHost();
            //GuestManagers.Object.NetworkEx.StartClient();

            yield return wait;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return null;

            if(HostManagers.Object.NetworkEx != null)
            {
                HostManagers.Object.NetworkEx.Shutdown();
                UnityEngine.Object.Destroy(hostNetwork);
            }
            HostManagers = null;

            if(GuestManagers.Object.NetworkEx != null)
            {
                GuestManagers.Object.NetworkEx.Shutdown();
                UnityEngine.Object.Destroy(guestNetwork);
            }
        }

        [UnityTest]
        public IEnumerator SetUpTest()
        {
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

            yield return wait;

            Debug.Log(HostManagers.Object.NetworkEx.LocalID);
            Debug.Log(GuestManagers.Object.NetworkEx.LocalID);

            Assert.IsTrue(HostManagers.Object.NetworkEx.IsHost);
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator TearDownTest()
        {
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

            yield return wait;

            Assert.IsTrue(HostManagers.Object.NetworkEx.IsHost);
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator OnConnected_Callback_Should_Invoke()
        {
            bool isConnected = false;

            HostManagers.Object.NetworkEx.OnConnect += () =>
            {
                isConnected = true;
            };

            HostManagers.Object.NetworkEx.StartHost();

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

            HostManagers.Object.NetworkEx.OnClientConnected += (clientID) =>
            {
                Debug.Log($"Client connected with ID: {clientID}");
                isClientConnected = true;
            };

            // Act
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

            yield return wait;

            // Assert
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient, "Guest is not properly connected.");
            Assert.IsTrue(isClientConnected, "OnClientConnected callback was not invoked.");
        }

        /// <summary>
        ///     네트워크 연결 전에도 CustomMessaging은 등록 가능해야 합니다.
        /// </summary>
        [UnityTest]
        public IEnumerator Subscribe_Message_Before_Start_Should_Register_After_Start()
        {
            // Arrange
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();
            yield return wait;

            GuestManagers.Object.NetworkEx.SubscribeMessage("TestMessage", (clientID, reader) =>
            {
                Debug.Log($"Test Message ClientID : {clientID}");
            });
            
            var targetID = GuestManagers.Object.NetworkEx.LocalID;

            // Act
            HostManagers.Object.NetworkEx.SendMessage("TestMessage", targetID, (writer) =>
            {
                Debug.Log("Test Message");
            },
            NetworkDelivery.ReliableSequenced);

            yield return wait;

            LogAssert.Expect(LogType.Log, $"Test Message ClientID : {HostManagers.Object.NetworkEx.LocalID}");
        }

        [UnityTest]
        public IEnumerator Host_Send_Client_Should_Received()
        {
            // Arrange
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

            yield return wait;

            bool expectedBool = false;
            string expectedString = "TestMessage";
            int expectedInt = 1234;

            ulong targetID = GuestManagers.Object.NetworkEx.LocalID;

            string receivedString = "";
            int receivedInt = 0;

            GuestManagers.Object.NetworkEx.SubscribeMessage("Host", (senderID, reader) =>
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
            HostManagers.Object.NetworkEx.SendMessage("Host", targetID, messageWriter, NetworkDelivery.Reliable);

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