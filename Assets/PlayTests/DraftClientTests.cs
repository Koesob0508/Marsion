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

            mockManagers.Setup(m => m.Network).Returns(mockNetworkEx.Object);

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

        [UnityTest]
        public IEnumerator DraftClient_Should_Receives()
        {
            var defaultFactory = new DefaultManagersFactory();
            var mockWrapper = defaultFactory.CreateNetworkWrapper();
            var mockNetworkEx = defaultFactory.CreateNetworkEx(mockWrapper);

            yield return null;

            var mockManagers = new Mock<IManagers>();

            mockManagers.Setup(m => m.Network).Returns(mockNetworkEx);

            var draftClient = new DraftClient(mockManagers.Object);

            draftClient.Init();

            mockManagers.Object.Network.StartHost();

            yield return new WaitForSeconds(0.5f);

            // 이제 mockManagers.NetworkEx를 통해서 Send를 해봐야지
            ushort testCommand = DraftMessageCode.ServerInitState;
            ulong targetID = mockManagers.Object.Network.LocalID;
            SerializedDraftState sdata = new SerializedDraftState();
            sdata.isComplete = true;
            sdata.count = 0;
            sdata.portraitID = "";
            sdata.deck = new string[0];
            sdata.selections = new string[0];
            sdata.subSelections = new string[0];

            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(testCommand);
                writer.WriteNetworkSerializable(sdata);
            };

            mockManagers.Object.Network.SendMessage("DraftServer", targetID, writeAction, NetworkDelivery.ReliableSequenced);

            yield return new WaitForSeconds(0.5f);

            LogAssert.Expect(LogType.Log, $"[DraftClient] <color={ColorCodes.Client}><b>Received init state</b></color>");
        }
    }
}