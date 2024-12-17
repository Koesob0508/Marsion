using Moq;
using NUnit.Framework;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Marsion.Tests
{
    public class DraftClientTests
    {
        /*
         * Draft Client 의존성
         * NetworkEx
         * UI
         */

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            if (SceneManager.GetActiveScene().name != "TestScene")
            {
                yield return SceneManager.LoadSceneAsync("TestScene");
            }
        }

        [UnityTest]
        public IEnumerator DraftClient_Init_Should_Subscribe()
        {
            // Arrange
            var mockManagers = new Mock<IManagers>();
            var mockNetworkEx = new Mock<INetworkManagerEx>();

            // Mock factory methods to return placeholders.

            mockManagers.Setup(m => m.NetworkEx).Returns(mockNetworkEx.Object);

            var draftClient = new DraftClient(mockManagers.Object);

            // Act
            draftClient.Init();

            // Assert
            mockNetworkEx.Verify(
                net => net.SubscribeMessage(
                    It.Is<string>(s => s == "DraftServer"),
                    It.IsAny<Action<ulong, FastBufferReader>>()
                    ),
                Times.Once,
                "DraftClient Should subscribe to 'DraftServer' message during Init"
                );

            yield break;
        }

        // CustomMessaging을 사용하려면 StartHost 하셔야함.
        // 생각해보니 당연한거네
        [UnityTest]
        public IEnumerator DraftClient_Should_Receives()
        {
            var defaultFactory = new DefaultManagersFactory();
            var mockWrapper = defaultFactory.CreateNetworkManagerWrapper();
            var mockNetworkEx = defaultFactory.CreateNetworkManagerEx(mockWrapper);

            yield return null;

            var mockManagers = new Mock<IManagers>();

            mockManagers.Setup(m => m.NetworkEx).Returns(mockNetworkEx);

            if(mockManagers.Object.NetworkEx.CustomMessagingManager == null) Debug.Log("CM null");

            var draftClient = new DraftClient(mockManagers.Object);

            draftClient.Init();

            // 이제 mockManagers.NetworkEx를 통해서 Send를 해봐야지
            ushort testCommand = DraftCommand.ServerInitState;
            ulong targetID = mockManagers.Object.NetworkEx.LocalID;
            SerializedDraftState sdata = null;

            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(testCommand);
                writer.WriteNetworkSerializable(sdata);
            };

            mockManagers.Object.NetworkEx.SendMessage("DraftServer", targetID, writeAction, NetworkDelivery.ReliableSequenced);

            yield return new WaitForSeconds(0.5f);

            LogAssert.Expect(LogType.Log, $"[DraftClient] <color={ColorCodes.Client}><b>Received init state</b></color>");

            yield break;
        }
    }
}