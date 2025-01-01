using Marsion.UI;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

namespace Marsion
{
    public class DraftClient
    {
        private readonly IManagers _managers;

        private Dictionary<ushort, Action<SerializedData>> _commands = new();
        public DraftState State { get; private set; }

        // shortcuts
        private bool IsHost { get { return _managers.Network.IsHost; } }
        private ulong ServerID { get { return _managers.Network.ServerID; } }

        public Action OnStateUpdate;

        public DraftClient(IManagers managers)
        {
            _managers = managers ?? throw new ArgumentNullException(nameof(managers));
        }

        public void Init()
        {
            Logger.Log<DraftClient>($"Draft Client initialized", colorName: ColorCodes.Client);

            RegisterCommand(DraftMessageCode.ServerInitState, OnReceiveInitState);
            RegisterCommand(DraftMessageCode.ServerStartDraft, OnReceiveStartDraft);
            RegisterCommand(DraftMessageCode.ServerUpdateState, OnReceiveUpdateState);

            _managers.Network.SubscribeMessage("DraftServer", OnReceivedCommand);
        }

        private void RegisterCommand(ushort tag, Action<SerializedData> callback)
        {
            if(!_commands.TryAdd(tag, callback))
            {
                Logger.LogWarning<DraftClient>($"Command {tag} is already registered.");
            }
        }

        private void OnReceivedCommand(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort tag);  
            SerializedData sdata = new SerializedData(reader);
            ExecuteCommand(tag, sdata);
        }

        private void ExecuteCommand(ushort tag, SerializedData sdata)
        {
            if(_commands.TryGetValue(tag, out var command))
            {
                command.Invoke(sdata);
            }
            else
            {
                Logger.LogWarning<DraftClient>($"Unknown command received: {tag}");
            }
        }

        #region OnReceive
        private void OnReceiveInitState(SerializedData sdata)
        {
            Logger.Log<DraftClient>($"Received init state", colorName: ColorCodes.Client);

            var sState = sdata.Get<SerializedDraftState>();
            State = new DraftState(sState);
        }

        private void OnReceiveStartDraft(SerializedData sdata)
        {
            Logger.Log<DraftClient>($"Received start draft", colorName: ColorCodes.Client);

            _managers.UI.ShowPopupUI<UI_DraftPanel>();
        }

        private void OnReceiveUpdateState(SerializedData sdata)
        {
            Logger.Log<DraftClient>($"Received updated state", colorName: ColorCodes.Client);

            var sState = sdata.Get<SerializedDraftState>();
            State = new DraftState(sState);

            OnStateUpdate?.Invoke();
        }

        #endregion

        #region Send

        public void RequestStartDraft()
        {
            Send(DraftMessageCode.ClientStartDraft);
        }

        public void Select(int index)
        {
            SendSelect(index);
        }

        public void Ready()
        {
            Send(DraftMessageCode.ClientReady);
        }

        private void SendSelect(int index)
        {
            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(DraftMessageCode.ClientSelect);
                writer.WriteValueSafe(index);
            };

            _managers.Network.SendMessage("DraftClient", ServerID, writeAction, NetworkDelivery.ReliableSequenced);
        }

        private void Send(ushort tag)
        {
            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(tag);
            };

            _managers.Network.SendMessage("DraftClient", ServerID, writeAction, NetworkDelivery.ReliableSequenced);
        }

        private void Send<T>(ushort tag, T data, NetworkDelivery delivery) where T : INetworkSerializable
        {
            Action<FastBufferWriter> writeAction = (writer) =>
            {
                writer.WriteValueSafe(tag);
                writer.WriteNetworkSerializable(data);
            };

            _managers.Network.SendMessage("DraftClient", ServerID, writeAction, NetworkDelivery.ReliableSequenced);
        }

        #endregion
    }
}