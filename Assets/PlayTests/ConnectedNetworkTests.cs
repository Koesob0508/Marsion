using NUnit.Framework;
using System.Collections;
using Unity.Netcode;
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
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            yield return null;
        }

        [UnityTest]
        public IEnumerator ConnectedTest()
        {
            yield return null;
        }
    }
}