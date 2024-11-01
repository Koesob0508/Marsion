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
        private bool IsHost { get { return Managers.Network.IsHost; } }
        private ulong ServerID { get { return Managers.Network.ServerID; } }
        private NetworkMessaging Messaging { get { return Managers.Network.Messaging; } }

        public Action OnStateUpdate;

        public void Init()
        {
            Managers.Logger.Log<DraftClient>($"Draft Client initialized", colorName: ColorCodes.Client);

            Commands = new();

            RegisterCommand(DraftCommand.ServerInitState, OnReceiveInitState);
            RegisterCommand(DraftCommand.ServerStartDraft, OnReceiveStartDraft);
            RegisterCommand(DraftCommand.ServerUpdateState, OnReceiveUpdateState);

            Managers.Network.Messaging.SubscribeMessage("DraftServer", OnReceiveCommand);
        }

        private void RegisterCommand(ushort tag, Action<SerializedData> callback)
        {
            Commands.Add(tag, callback);
        }

        private void OnReceiveCommand(ulong clientID, FastBufferReader reader)
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
            Managers.Logger.Log<DraftClient>($"Received init state", colorName: ColorCodes.Client);

            SerializedDraftState sState = sdata.Get<SerializedDraftState>();

            State = new DraftState(sState);
        }

        private void OnReceiveStartDraft(SerializedData sdata)
        {
            Managers.Logger.Log<DraftClient>($"Received start draft", colorName: ColorCodes.Client);

            Managers.UI.ShowPopupUI<UI_DraftPanel>();
        }

        private void OnReceiveUpdateState(SerializedData sdata)
        {
            Managers.Logger.Log<DraftClient>($"Received updated state", colorName: ColorCodes.Client);

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
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(DraftCommand.ClientSelect);
            writer.WriteValueSafe(index);
            Messaging.Send("DraftClient", ServerID, writer, NetworkDelivery.ReliableSequenced);
            writer.Dispose();
        }

        private void Send(ushort tag)
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(tag);
            Messaging.Send("DraftClient", ServerID, writer, NetworkDelivery.Reliable);
            writer.Dispose();
        }

        private void Send<T>(ushort tag, T data, NetworkDelivery delivery) where T : INetworkSerializable
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(tag);
            writer.WriteNetworkSerializable(data);
            Messaging.Send("DraftClient", ServerID, writer, delivery);
            writer.Dispose();
        }

        #endregion
    }
}