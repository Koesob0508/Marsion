using Moq;
using NUnit.Framework;
using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    public class ConnectedNetworkTests
    {
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

            yield return new WaitForSeconds(0.5f);
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return null;

            if(HostManagers.Object.NetworkEx != null)
            {
                HostManagers.Object.NetworkEx.Shutdown();
                Object.Destroy(hostNetwork);
            }
            HostManagers = null;

            if(GuestManagers.Object.NetworkEx != null)
            {
                GuestManagers.Object.NetworkEx.Shutdown();
                Object.Destroy(guestNetwork);
            }
        }

        [UnityTest]
        public IEnumerator SetUpTest()
        {
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

            yield return new WaitForSeconds(0.5f);

            Debug.Log(HostManagers.Object.NetworkEx.LocalClientID);
            Debug.Log(GuestManagers.Object.NetworkEx.LocalClientID);

            Assert.IsTrue(HostManagers.Object.NetworkEx.IsHost);
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator TearDownTest()
        {
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(HostManagers.Object.NetworkEx.IsHost);
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator OnClientConnected_Callback_Should_Invoke()
        {
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

            yield return new WaitForSeconds(1f);

            HostManagers.Object.NetworkEx.StartHost();

            yield return new WaitForSeconds(1f);

            GuestManagers.Object.NetworkEx.StartClient();

            yield return new WaitForSeconds(1f);

            //var connectedClients = HostManagers.Object.NetworkEx.ConnectedClientsIDs;
            //foreach (var clientId in connectedClients)
            //{
            //    Debug.Log($"Existing connected client ID: {clientId}");
            //    isClientConnected = true; // 이미 연결된 클라이언트를 검증
            //}

            // 검증
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient, "Guest is not properly connected.");
            Assert.IsTrue(isClientConnected, "OnClientConnected callback was not invoked.");
        }

        [UnityTest]
        public IEnumerator Host_Send_Client_Should_Received()
        {
            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

            bool expectedBool = false;
            string expectedString = "TestMessage";
            int expectedInt = 1234;

            yield return new WaitForSeconds(0.5f);

            ulong targetID = GuestManagers.Object.NetworkEx.LocalClientID;

            string receivedString = "";
            int receivedInt = 0;

            GuestManagers.Object.NetworkEx.SubscribeMessage("Host", (senderID, reader) =>
            {
                expectedBool = true;
                reader.ReadValueSafe(out receivedString);
                reader.ReadValueSafe(out receivedInt);
            });

            System.Action<FastBufferWriter> messageWriter = (writer) =>
            {
                writer.WriteValueSafe(expectedString);
                writer.WriteValueSafe(expectedInt);
            };

            HostManagers.Object.NetworkEx.SendMessage("Host", targetID, messageWriter, NetworkDelivery.Reliable);

            yield return new WaitForSeconds(0.5f); // 메시지 처리 시간 대기

            Assert.IsTrue(expectedBool);
            Assert.AreEqual(expectedString, receivedString);
            Assert.AreEqual(expectedInt, receivedInt);

            yield break;
        }
    }
}