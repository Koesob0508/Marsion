using Marsion.CardView;
using Marsion.Client;
using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion
{
    public class GameClientEx : MonoBehaviour, IGameClient
    {
        [SerializeField] Sequencer Upstream;
        [SerializeField] Sequencer Downstream;
        [SerializeField] HeroView PlayerHero;
        [SerializeField] HeroView EnemyHero;

        [SerializeField] HandView hand;
        [SerializeField] FieldView playerField;
        [SerializeField] FieldView enemyField;

        private Dictionary<ushort, Action<SerializedData>> Commands;

        public GameData Data { get; private set; }
        public ulong ServerID => Managers.Network.ServerID;
        public ulong PlayerID => Managers.Network.ClientID;
        public ulong EnemyID { get; private set; }
        private NetworkMessaging Messaging => Managers.Network.Messaging;

        public IHandView Hand => hand;

        public IFieldView PlayerField => playerField;

        public IFieldView EnemyField => enemyField;

        public event Action OnDataUpdated;
        public event Action OnGameStarted;
        public event Action OnGameReset;
        public event Action OnTurnStarted;
        public event Action OnTurnEnded;
        public event Action OnManaChanged;
        public event Action<Player, Card> OnCardDrawn;
        public event Action<bool, Player, Card> OnCardPlayed;
        public event Action<bool, Player, Card, int> OnCardSpawned;
        public event Action<Sequencer.Sequence, Player, Card, Player, Card> OnStartAttack;

        public void Init()
        {
            Commands = new();
            Upstream.Init();
            Downstream.Init();

            RegisterCommand(GameCommand.ServerUpdateData, OnReceivedUpdatData);
            RegisterCommand(GameCommand.ServerStartGame, OnReceivedStartGame);
            RegisterCommand(GameCommand.ServerChangeMana, OnReceivedChangeMana);
            RegisterCommand(GameCommand.ServerStartTurn, OnReceivedStartTurn);
            RegisterCommand(GameCommand.ServerDrawCard, OnReceivedDrawCard);
            RegisterCommand(GameCommand.ServerPlayCardResult, OnReceivedPlayCardResult);
            RegisterCommand(GameCommand.ServerSpawnCardResult, OnReceivedSpawnCardResult);

            Messaging.SubscribeMessage("GameServer", OnReceivedCommand);
        }

        private void Clear()
        {

        }

        private void RegisterCommand(ushort type, Action<SerializedData> callback)
        {
            Commands.Add(type, callback);
        }

        private void OnReceivedCommand(ulong clientID, FastBufferReader reader)
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

        #region Send Operations

        public void SendTurnEnd()
        {

        }

        public void SendTryAttack(Card attacker, Card defender)
        {
            // Send Attack
        }

        public void SendTrySpawnCard(Card card, int index)
        {
            Managers.Logger.Log<GameClientEx>("Try spawn", colorName: ColorCodes.Client);

            SerializedTrySpawnCardData sdata = new SerializedTrySpawnCardData();
            sdata.CardUID = card.UID;
            sdata.Index = index;

            Send(GameCommand.ClientTrySpawnCard, sdata, NetworkDelivery.Reliable);
        }

        #endregion

        #region OnReceived

        private void OnReceivedUpdatData(SerializedData sdata)
        {
            SerializedGameData sGameData = sdata.Get<SerializedGameData>();

            Sequencer.Sequence sequence = new("UpdateData", Downstream);
            Sequencer.Clip clip = new("Update", sequence);

            clip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClientEx>($"Received updated data", colorName: ColorCodes.Client);

                Data = sGameData.GameData;
                Managers.Logger.Log<GameClientEx>($"{Data.GetPlayer(PlayerID).Mana} / {Data.GetPlayer(PlayerID).MaxMana}", colorName: ColorCodes.Client);

                OnDataUpdated?.Invoke();
            };
        }

        private void OnReceivedStartGame(SerializedData sdata)
        {
            Sequencer.Sequence Sequence = new("StartGame", Downstream);
            Sequencer.Clip initHeroClip = new("InitHero", Sequence);
            Sequencer.Clip initHandClip = new("InitHand", Sequence);
            Sequencer.Clip invokeClip = new("InvokeAction", Sequence);

            initHeroClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClientEx>($"Received start game", colorName: ColorCodes.Client);

                foreach (var player in Data.Players)
                {
                    if (PlayerID == player.PlayerID)
                    {
                        PlayerHero.Init(player.Card);
                        PlayerHero.Spawn();
                    }
                    else
                    {
                        EnemyID = player.PlayerID;
                        EnemyHero.Init(player.Card);
                        EnemyHero.Spawn();
                    }
                }
            };

            initHandClip.OnPlay += () =>
            {
                foreach (var player in Data.Players)
                {
                    foreach (var card in player.Hand)
                    {
                        OnCardDrawn?.Invoke(player, card);
                    }
                }
            };

            invokeClip.OnPlay += () =>
            {
                OnGameStarted?.Invoke();
            };
        }

        private void OnReceivedChangeMana(SerializedData sdata)
        {
            Sequencer.Sequence sequence = new("Change mana", Downstream);
            Sequencer.Clip clip = new("Change mana", sequence);

            clip.OnPlay += () =>
            {
                OnManaChanged?.Invoke();
            };
        }

        private void OnReceivedStartTurn(SerializedData sdata)
        {
            Sequencer.Sequence sequence = new("Start turn", Downstream);
            Sequencer.Clip clip = new("Start turn", sequence);

            clip.OnPlay += () =>
            {
                OnTurnStarted?.Invoke();
            };
        }

        private void OnReceivedDrawCard(SerializedData sdata)
        {
            SerializedDrawnCardData sCardData = sdata.Get<SerializedDrawnCardData>();

            Sequencer.Sequence sequence = new("Draw card", Downstream);
            Sequencer.Clip clip = new("Draw card", sequence);

            clip.OnPlay += () =>
            {
                OnCardDrawn?.Invoke(Data.GetPlayer(sCardData.PlayerID), Data.GetHandCard(sCardData.PlayerID, sCardData.CardUID));
            };
        }

        private void OnReceivedPlayCardResult(SerializedData sdata)
        {
            SerializedPlayCardResultData sResultData = sdata.Get<SerializedPlayCardResultData>();

            Sequencer.Sequence sequence = new("Play card result", Downstream);
            Sequencer.Clip clip = new("Play card result", sequence);
            ;

            clip.OnPlay += () =>
            {
                OnCardPlayed?.Invoke(sResultData.Succeeded, Data.GetPlayer(sResultData.PlayerID), Data.GetHandCard(sResultData.PlayerID, sResultData.CardUID));
            };
        }

        private void OnReceivedSpawnCardResult(SerializedData sdata)
        {
            SerializedSpawnCardResultData sResultData = sdata.Get<SerializedSpawnCardResultData>();

            Sequencer.Sequence sequence = new("Spawn card result", Downstream);
            Sequencer.Clip clip = new("Spawn card result", sequence);

            clip.OnPlay += () =>
            {
                OnCardSpawned?.Invoke(sResultData.Succeeded, Data.GetPlayer(sResultData.PlayerID), Data.GetHandCard(sResultData.PlayerID, sResultData.CardUID), sResultData.Index);
            };
        }

        #endregion

        #region Get Operations

        public bool IsMine(ulong id)
        {
            return PlayerID == id;
        }

        public bool IsMine(Player player)
        {
            return IsMine(player.PlayerID);
        }

        public bool IsMine(Card card)
        {
            return IsMine(card.PlayerID);
        }

        public bool IsMyTurn()
        {
            return IsMine(Data.CurrentPlayer);
        }

        public Card GetCard(CardType type, ulong playerID, string cardUID)
        {
            Card result = null;

            switch (type)
            {
                case CardType.Hero:
                    result = Data.GetPlayer(playerID).Card;
                    break;
                case CardType.Field:
                    result = Data.GetFieldCard(playerID, cardUID);
                    break;
            }

            return result;
        }

        public ICharacterView GetCharacter(ulong playerID, string cardUID)
        {
            if (IsMine(playerID))
            {
                if (cardUID == PlayerHero.Card.UID)
                {
                    return PlayerHero;
                }
                else
                {
                    return PlayerField.GetCreature(Data.GetFieldCard(playerID, cardUID));
                }
            }
            else
            {
                if (cardUID == EnemyHero.Card.UID)
                {
                    return EnemyHero;
                }
                else
                {
                    return EnemyField.GetCreature(Data.GetFieldCard(playerID, cardUID));
                }
            }
        }

        #endregion

        #region Send Utilities

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