using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class ManagersTests
    {
        [UnityTest]
        public IEnumerator DataInitTest()
        {
            while(Managers.Data.CardDictionary == null || Managers.Data.CardDictionary.Count == 0)
            {
                yield return null;
            }

            Assert.IsNotNull(Managers.Data.CardDictionary, "CardDictionary is null.");
            Assert.Greater(Managers.Data.CardDictionary.Count, 0);
        }
    }
}