using Marsion.UI;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace Marsion
{
    public class DraftClient
    {
        private Dictionary<ushort, Action<SerializedData>> Commands;
        public DraftState State { get; private set; }

        // shortcuts
        private bool IsHost { get { return Managers.Instance.NetworkEx.IsHost; } }
        private ulong ServerID { get { return Managers.Instance.NetworkEx.ServerID; } }

        public Action OnStateUpdate;

        public void Init()
        {
            Logger.Log<DraftClient>($"Draft Client initialized", colorName: ColorCodes.Client);

            Commands = new();

            RegisterCommand(DraftCommand.ServerInitState, OnReceiveInitState);
            RegisterCommand(DraftCommand.ServerStartDraft, OnReceiveStartDraft);
            RegisterCommand(DraftCommand.ServerUpdateState, OnReceiveUpdateState);

            Managers.Instance.NetworkEx.SubscribeMessage("DraftServer", OnReceivedCommand);
        }

        private void RegisterCommand(ushort tag, Action<SerializedData> callback)
        {
            Commands.Add(tag, callback);
        }

        private void OnReceivedCommand(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort tag);
            SerializedData sdata = new SerializedData(reader);
            ExecuteCommand(tag, sdata);
        }

        private void ExecuteCommand(ushort tag, SerializedData sdata)
        {
            bool found = Commands.TryGetValue(tag, out var command);
            if (found)
                command.Invoke(sdata);
        }

        #region OnReceive
        private void OnReceiveInitState(SerializedData sdata)
        {
            Logger.Log<DraftClient>($"Received init state", colorName: ColorCodes.Client);

            SerializedDraftState sState = sdata.Get<SerializedDraftState>();

            State = new DraftState(sState);
        }

        private void OnReceiveStartDraft(SerializedData sdata)
        {
            Logger.Log<DraftClient>($"Received start draft", colorName: ColorCodes.Client);

            Managers.Instance.UI.ShowPopupUI<UI_DraftPanel>();
        }

        private void OnReceiveUpdateState(SerializedData sdata)
        {
            Logger.Log<DraftClient>($"Received updated state", colorName: ColorCodes.Client);

            SerializedDraftState sState = sdata.Get<SerializedDraftState>();

            State = new DraftState(sState);

            OnStateUpdate?.Invoke();
        }

        #endregion

        #region Send

        public void RequestStartDraft()
        {
            Send(DraftCommand.ClientStartDraft);
        }

        public void Select(int index)
        {
            SendSelect(index);
        }

        public void Ready()
        {
            Send(DraftCommand.ClientReady);
        }

        private void SendSelect(int index)
        {
            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(DraftCommand.ClientSelect);
                writer.WriteValueSafe(index);
            };

            Managers.Instance.NetworkEx.SendMessage("DraftClient", ServerID, writeAction, NetworkDelivery.ReliableSequenced);
        }

        private void Send(ushort tag)
        {
            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(tag);
            };

            Managers.Instance.NetworkEx.SendMessage("DraftClient", ServerID, writeAction, NetworkDelivery.ReliableSequenced);
        }

        private void Send<T>(ushort tag, T data, NetworkDelivery delivery) where T : INetworkSerializable
        {
            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(tag);
                writer.WriteNetworkSerializable(data);
            };

            Managers.Instance.NetworkEx.SendMessage("DraftClient", ServerID, writeAction, NetworkDelivery.ReliableSequenced);
        }

        #endregion
    }
}