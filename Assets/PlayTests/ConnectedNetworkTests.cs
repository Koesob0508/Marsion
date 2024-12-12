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
                guestTransport.ConnectionData.Port = 7778;

                guestNetwork.NetworkConfig = new NetworkConfig
                {
                    NetworkTransport = guestTransport
                };
            }

            hostNetwork.StartHost();
            guestNetwork.StartClient();
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return null;

            if(hostNetwork != null)
            {
                hostNetwork.Shutdown();
                Object.Destroy(hostNetwork);
            }

            if(guestNetwork != null)
            {
                guestNetwork.Shutdown();
                Object.Destroy(guestNetwork);
            }
        }

        [UnityTest]
        public IEnumerator SetUpTest()
        {
            yield return null;

            Assert.IsTrue(hostNetwork.IsHost);
            Assert.IsTrue(guestNetwork.IsClient);

            yield break;
        }

        [UnityTest]
        public IEnumerator TearDownTest()
        {
            yield return null;

            Assert.IsTrue(hostNetwork.IsHost);
            Assert.IsTrue(guestNetwork.IsClient);

            yield break;
        }
    }
}