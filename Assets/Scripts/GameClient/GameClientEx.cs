using Marsion.CardView;
using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class GameClientEx : MonoBehaviour
    {
        [SerializeField] Sequencer Sequencer;
        [SerializeField] HeroView PlayerHero;
        [SerializeField] HeroView EnemyHero;

        private Dictionary<ushort, Action<SerializedData>> Commands;
        private ulong ClientID { get { return Managers.Network.ClientID; } }
        private ulong ServerID { get { return Managers.Network.ServerID; } }
        private NetworkMessaging Messaging { get { return Managers.Network.Messaging; } }

        public GameData Data { get; private set; }

        public event Action OnDataUpdated;
        public event Action OnGameStarted;
        public event Action<Player, Card> OnCardDrawn;

        public void Init()
        {
            Commands = new();
            Sequencer.Init();

            RegisterCommand(GameCommand.ServerUpdateData, OnReceiveUpdatData);
            RegisterCommand(GameCommand.ServerStartGame, OnReceiveStartGame);

            Messaging.SubscribeMessage("GameServer", OnReceiveCommand);
        }

        private void Clear()
        {

        }

        private void RegisterCommand(ushort type, Action<SerializedData> callback)
        {
            Commands.Add(type, callback);
        }

        private void OnReceiveCommand(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort type);
            SerializedData sdata = new SerializedData(reader);
            ExecuteCommand(type, sdata);
        }

        private void ExecuteCommand(ushort type, SerializedData sdata)
        {
            bool found = Commands.TryGetValue(type, out var command);
            if (found)
                command.Invoke(sdata);
        }

        #region Operations

        #endregion

        #region OnReceive

        private void OnReceiveUpdatData(SerializedData sdata)
        {
            Managers.Logger.Log<GameClientEx>($"Received updated data", colorName: ColorCodes.Client);
            SerializedGameData sGameData = sdata.Get<SerializedGameData>();
            Data = sGameData.gameData;

            OnDataUpdated?.Invoke();

            //Sequencer.Sequence Sequence = new("UpdateData", Sequencer);
            //Sequencer.Clip logClip = new("Log");
            //Sequencer.Clip updateClip = new("Update");

            //logClip.OnPlay += () =>
            //{
            //    Managers.Logger.Log<GameClientEx>($"Received updated data", colorName: ColorCodes.Client);
            //};

            //updateClip.OnPlay += () =>
            //{
            //    SerializedGameData sGameData = sdata.Get<SerializedGameData>();
            //    Data = sGameData.gameData;

            //    OnDataUpdated?.Invoke();
            //};

            //Sequence.Append(logClip);
            //Sequence.Append(updateClip);
            //Sequencer.Append(Sequence);
        }

        private void OnReceiveStartGame(SerializedData sdata)
        {
            Managers.Logger.Log<GameClientEx>($"Received start game", colorName: ColorCodes.Client);

            foreach (var player in Data.Players)
            {
                if (ClientID == player.ClientID)
                {
                    PlayerHero.Init(player.Card);
                    PlayerHero.Spawn();
                }
                else
                {
                    EnemyHero.Init(player.Card);
                    EnemyHero.Spawn();
                }
            }

            foreach (var player in Data.Players)
            {
                foreach (var card in player.Hand)
                {
                    OnCardDrawn?.Invoke(player, card);
                }
            }
            OnGameStarted?.Invoke();

            //Sequencer.Sequence Sequence = new("StartGame", Sequencer);
            //Sequencer.Clip logClip = new("Log");
            //Sequencer.Clip initHeroClip = new("InitHero");
            //Sequencer.Clip initHandClip = new("InitHand");
            //Sequencer.Clip invokeClip = new("InvokeAction");

            //logClip.OnPlay += () =>
            //{
            //    Managers.Logger.Log<GameClientEx>($"Received start game", colorName: ColorCodes.Client);
            //};

            //initHeroClip.OnPlay += () =>
            //{
            //    foreach(var player in Data.Players)
            //    {
            //        if(ClientID == player.ClientID)
            //        {
            //            PlayerHero.Init(player.Card);
            //            PlayerHero.Spawn();
            //        }
            //        else
            //        {
            //            EnemyHero.Init(player.Card);
            //            EnemyHero.Spawn();
            //        }
            //    }
            //};

            //initHandClip.OnPlay += () =>
            //{
            //    foreach (var player in Data.Players)
            //    {
            //        foreach(var card in player.Hand)
            //        {
            //            OnCardDrawn?.Invoke(player, card);
            //        }
            //    }
            //};

            //invokeClip.OnPlay += () =>
            //{
            //    OnGameStarted?.Invoke();
            //};

            //Sequence.Append(logClip);
            //Sequence.Append(initHeroClip);
            //Sequence.Append(initHandClip);
            //Sequence.Append(invokeClip);
            //Sequencer.Append(Sequence);
        }

        #endregion

        #region Send

        private void Send(ushort type)
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(type);
            Messaging.Send("GameClient", ServerID, writer, NetworkDelivery.Reliable);
            writer.Dispose();
        }

        private void Send<T>(ushort type, T data, NetworkDelivery delivery) where T : INetworkSerializable
        {
            FastBufferWriter writer = new FastBufferWriter(128, Allocator.Temp, MarsNetwork.MessageSizeMax);
            writer.WriteValueSafe(type);
            writer.WriteNetworkSerializable(data);
            Messaging.Send("GameClient", ServerID, writer, delivery);
            writer.Dispose();
        }

        #endregion
    }
}