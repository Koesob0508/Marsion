using Moq;
using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    [TestFixture]
    public class ManagersTests
    {
        private const string TestSceneName = "TestScene";

        [UnityTest]
        public IEnumerator ManagersInitializer_SetsManagersInstance_WhenSceneLoads()
        {
            var mockFactory = new Mock<IManagersFactory>();
            Managers.Init(mockFactory.Object);

            Assert.IsNotNull(Managers.Instance);

            yield break;
        }
    }
}