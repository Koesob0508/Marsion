using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class GameServerEx : MonoBehaviour
    {
        [SerializeField] Sequencer Sequencer;
        private Dictionary<ushort, Action<ulong, SerializedData>> Commands;
        private GameData Data => Logic.Data;
        private GameLogicEx Logic;

        private List<ulong> ConnectedClients;

        private NetworkMessaging Messaging { get { return Managers.Network.Messaging; } }

        public void Init()
        {
            Managers.Logger.Log<GameServerEx>($"Game Server initialized", colorName: ColorCodes.Server);

            Commands = new();
            Logic = new GameLogicEx(new GameData(2));
            Sequencer.Init();

            ConnectedClients = new List<ulong>();

            Logic.OnDataUpdated += SendUpdateData;
            Logic.OnGameStarted += SendStartGame;

            Managers.Network.Messaging.SubscribeMessage("GameClient", OnReceiveCommand);
        }

        private void Reset()
        {

        }

        public void Clear()
        {
            Logic.OnDataUpdated -= SendUpdateData;
        }

        #region Operations

        public void Ready(ulong clientID, List<string> deck)
        {
            ConnectedClients.Add(clientID);
            Logic.SetPlayerDeck(clientID, deck);
            if (ConnectedClients.Count == 2)
            {
                StartGame();
            }

            //Sequencer.Sequence Sequence = new Sequencer.Sequence("Ready", Sequencer);
            //Sequencer.Clip SetDeckClip = new Sequencer.Clip("SetDeck");
            //Sequencer.Clip CheckClip = new Sequencer.Clip("Check");

            //SetDeckClip.OnPlay += () =>
            //{
            //    ConnectedClients.Add(clientID);
            //    Logic.SetPlayerDeck(clientID, deck);
            //};

            //CheckClip.OnPlay += () =>
            //{
            //    if (ConnectedClients.Count == 2)
            //    {
            //        StartGame();
            //    }
            //};

            //Sequence.Append(SetDeckClip);
            //Sequence.Append(CheckClip);

            //Sequencer.Append(Sequence);
        }

        public void StartGame()
        {
            Managers.Logger.Log<GameServerEx>($"Start game", colorName: ColorCodes.Server);
            Logic.StartGame();

            //Sequencer.Sequence Sequence = new("StartGame", Sequencer);
            //Sequencer.Clip LogClip = new("Log");
            //Sequencer.Clip StartGameClip = new("StartGame");

            //LogClip.OnPlay += () =>
            //{
            //    Managers.Logger.Log<GameServerEx>($"Start game", colorName: ColorCodes.Server);
            //};

            //StartGameClip.OnPlay += () =>
            //{
            //    Logic.StartGame();
            //};

            //Sequence.Append(LogClip);
            //Sequence.Append(StartGameClip);
            //Sequencer.Append(Sequence);

            // StartGame

            // Logic.Start Turn
        }

        #endregion

        private void RegisterCommand(ushort type, Action<ulong, SerializedData> callback)
        {
            Commands.Add(type, callback);
        }

        private void OnReceiveCommand(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort type);
            SerializedData sdata = new SerializedData(reader);
            ExecuteCommand(type, clientID, sdata);
        }

        private void ExecuteCommand(ushort type, ulong clientID, SerializedData sdata)
        {
            bool found = Commands.TryGetValue(type, out var command);
            if (found)
                command.Invoke(clientID, sdata);
        }

        #region OnReceive

        #endregion

        #region Send

        private void SendUpdateData()
        {
            Managers.Logger.Log<GameServerEx>($"Send updated data", colorName: ColorCodes.Server);
            SerializedGameData sdata = new SerializedGameData();
            sdata.gameData = new GameData(Data);

            SendToAll(GameCommand.ServerUpdateData, sdata, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void SendStartGame()
        {
            Managers.Logger.Log<GameServerEx>($"Send updated data", colorName: ColorCodes.Server);
            SendToAll(GameCommand.ServerStartGame);
        }

        private void Send(ulong target, ushort tag)
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(tag);
            Managers.Network.Messaging.Send("GameServer", target, writer, NetworkDelivery.ReliableSequenced);
            writer.Dispose();
        }

        private void Send(ulong target, ushort tag, INetworkSerializable data, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(tag);
            writer.WriteNetworkSerializable(data);
            Managers.Network.Messaging.Send("GameServer", target, writer, delivery);
            writer.Dispose();
        }


        private void SendToAll(ushort tag)
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(tag);
            foreach(ulong clientID in ConnectedClients)
            {
                Messaging.Send("GameServer", clientID, writer, NetworkDelivery.ReliableSequenced);
            }
            writer.Dispose();
        }

        private void SendToAll(ushort tag, string data, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(tag);
            writer.WriteValueSafe(data);
            foreach(var clientID in ConnectedClients)
            {
                Messaging.Send("GameServer", clientID, writer, delivery);
            }
            writer.Dispose();
        }

        private void SendToAll(ushort tag, INetworkSerializable data, NetworkDelivery delivery)
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(tag);
            writer.WriteNetworkSerializable(data);
            foreach(var clientID in ConnectedClients)
            {
                Messaging.Send("GameServer", clientID, writer, delivery);
            }
            writer.Dispose();
        }

        #endregion
    }
}