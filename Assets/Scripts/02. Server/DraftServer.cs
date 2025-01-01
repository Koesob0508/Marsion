using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public enum SelectType
    {
        Legendary = 0,
        Table = 1,
        Exchange = 2
    }

    public class DraftServer
    {
        private Dictionary<ushort, Action<ulong, SerializedData>> Commands;
        private Dictionary<ulong, DraftState> DraftDictionary;
        private Queue<int> InitialTypeSequence;

        public Action OnUpdateDraftState;

        public void Init()
        {
            Logger.Log<DraftServer>($"Draft Server initialized", colorName: ColorCodes.Server);

            Commands = new();
            DraftDictionary = new();
            InitialTypeSequence = new Queue<int>(Enumerable.Concat(
                new[] { 0 },
                Enumerable.Repeat(1, 5)
            ));

            RegisterCommand(DraftMessageCode.ClientStartDraft, OnReceiveStartDraft);
            RegisterCommand(DraftMessageCode.ClientSelect, OnReceiveSelect);
            RegisterCommand(DraftMessageCode.ClientReady, OnReceiveReady);

            Managers.Instance.Network.SubscribeMessage("DraftClient", OnReceiveCommand);
        }

        public void AddState(ulong clientID)
        {
            if (!DraftDictionary.ContainsKey(clientID))
            {
                Logger.Log<DraftServer>($"Client(ID : {clientID}) regist", colorName: ColorCodes.Server);
                var state = new DraftState(InitialTypeSequence);

                // 임의로 직업 초상화 설정.
                // TODO : 추후에 직업 또한 선택지로 제시할 것
                System.Random random = new System.Random();
                int portraitID = random.Next(3, 13);
                state.SetPortrait(portraitID.ToString());

                state.SetSelection();
                DraftDictionary.Add(clientID, state);
            }
            else
            {
                Logger.Log<DraftServer>($"Client(ID : {clientID}) Draft State already exists", colorName: ColorCodes.Server);
            }
        }

        private void RegisterCommand(ushort type, Action<ulong, SerializedData> callback)
        {
            Commands.Add(type, callback);
        }

        private void OnReceiveCommand(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort tag);
            SerializedData sdata = new SerializedData(reader);
            ExecuteCommand(tag, clientID, sdata);
        }

        private void ExecuteCommand(ushort tag, ulong clientID, SerializedData sdata)
        {
            bool found = Commands.TryGetValue(tag, out var command);
            if (found)
                command.Invoke(clientID, sdata);
        }

        #region OnReceive

        private void OnReceiveStartDraft(ulong clientID, SerializedData sdata)
        {
            Logger.Log<DraftServer>($"Start Draft from Client : {clientID}", colorName: ColorCodes.Server);

            SendInitState(clientID);
            SendStartDraft(clientID);
        }

        private void OnReceiveSelect(ulong clientID, SerializedData sdata)
        {
            Logger.Log<DraftServer>($"Select from Client : {clientID}", colorName: ColorCodes.Server);

            int index = sdata.GetInt();

            Logger.Log<DraftServer>($"Client {clientID} select {index}", colorName: ColorCodes.Server);

            if(DraftDictionary.TryGetValue(clientID, out var state))
            {
                state.Select(index);
            }
            else
            {
                Logger.LogWarning<DraftServer>($"Client {clientID} has not state", colorName: ColorCodes.Server);
            }

            SendUdpateState(clientID);
        }

        private void OnReceiveReady(ulong clientID, SerializedData sdata)
        {
            Logger.Log<DraftServer>($"Ready from Client : {clientID}", colorName: ColorCodes.Server);

            DraftDictionary.TryGetValue(clientID, out var state);

            Managers.Instance.Server.Ready(clientID, state);
        }

        #endregion

        #region Send

        private void SendInitState(ulong clientID)
        {
            if (DraftDictionary.TryGetValue(clientID, out var state))
            {
                SerializedDraftState sdata = new();
                sdata.isComplete = state.IsComplete;
                sdata.count = state.Count;
                sdata.portraitID = state.Portrait;
                sdata.deck = state.CurrentDeck.ToArray();
                sdata.selections = state.CurrentSelections.ToArray();
                sdata.subSelections = state.CurrentSubSelections.ToArray();

                Send(clientID, DraftMessageCode.ServerInitState, sdata, NetworkDelivery.ReliableSequenced);
            }
        }

        private void SendStartDraft(ulong clientID)
        {
            Send(clientID, DraftMessageCode.ServerStartDraft);
        }

        private void SendUdpateState(ulong clientID)
        {
            if (DraftDictionary.TryGetValue(clientID, out var state))
            {
                SerializedDraftState sdata = new();
                sdata.isComplete = state.IsComplete;
                sdata.count = state.Count;
                sdata.portraitID = state.Portrait;
                sdata.deck = state.CurrentDeck.ToArray();
                sdata.selections = state.CurrentSelections.ToArray();
                sdata.subSelections = state.CurrentSubSelections.ToArray();

                Send(clientID, DraftMessageCode.ServerUpdateState, sdata, NetworkDelivery.ReliableSequenced);
            }
            else
            {
                Logger.LogWarning<DraftServer>($"Client({clientID}) has not draft state", colorName: ColorCodes.Server);
            }
        }

        // Generic send
        private void Send(ulong target, ushort tag)
        {
            Managers.Instance.Network.SendMessage("DraftServer", target, (writer) =>
            {
                writer.WriteValueSafe(tag);
            }, NetworkDelivery.ReliableSequenced);
        }

        private void Send(ulong target, ushort tag, INetworkSerializable data, NetworkDelivery delivery)
        {
            Managers.Instance.Network.SendMessage("DraftServer", target, (writer) =>
            {
                writer.WriteValueSafe(tag);
                writer.WriteNetworkSerializable(data);
            }, NetworkDelivery.ReliableSequenced);
        }

        #endregion

        // Operations

        public bool GetDeck(ulong clientID, out List<string> draftedDeck)
        {
            if(DraftDictionary.TryGetValue(clientID, out var state))
            {
                draftedDeck = state.CurrentDeck;
                return true;
            }
            else
            {
                Logger.Log<DraftServer>($"Client({clientID}) has not state", colorName: ColorCodes.Server);
                draftedDeck = null;
                return false;
            }
        }
    }
}