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
            // Arrange
            LogAssert.ignoreFailingMessages = true; // Ignore failing log messages during the test

            // Act
            yield return SceneManager.LoadSceneAsync(TestSceneName);
            yield return null; // Wait for one frame to allow Start to execute

            // Assert
            Assert.IsTrue(GameObject.Find("@ManagersInitializer"));

            yield return null;

            Assert.IsNotNull(Managers.Instance, "Managers.Instance should not be null after ManagersInitializer.Start is called.");

            // Cleanup
            LogAssert.ignoreFailingMessages = false; // Restore the default behavior after the test
        }
    }
}