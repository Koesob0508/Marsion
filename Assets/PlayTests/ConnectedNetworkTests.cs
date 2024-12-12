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

            HostManagers.Object.NetworkEx.StartHost();
            GuestManagers.Object.NetworkEx.StartClient();

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
            yield return null;

            Debug.Log(HostManagers.Object.NetworkEx.LocalClientID);
            Debug.Log(GuestManagers.Object.NetworkEx.LocalClientID);

            Assert.IsTrue(HostManagers.Object.NetworkEx.IsHost);
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator TearDownTest()
        {
            yield return null;

            Assert.IsTrue(HostManagers.Object.NetworkEx.IsHost);
            Assert.IsTrue(GuestManagers.Object.NetworkEx.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator Host_Send_Client_Should_Received()
        {
            string expectedString = "TestMessage";
            int expectedInt = 1234;

            yield return null;

            
        }
    }
}