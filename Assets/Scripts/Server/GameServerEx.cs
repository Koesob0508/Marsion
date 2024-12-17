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
        [SerializeField] Sequencer Upstream;
        [SerializeField] Sequencer Downstream;

        private GameLogicEx Logic;
        private GameData Data => Logic.Data;

        private List<ulong> ConnectedClients;
        private Dictionary<ushort, Action<ulong, SerializedData>> Commands;

        public void Init()
        {
            Logger.Log<GameServerEx>($"Game Server initialized", colorName: ColorCodes.Server);

            ConnectedClients = new();
            Commands = new();

            Upstream.Init();
            Downstream.Init();

            Logic = new GameLogicEx(new GameData(2));

            RegisterCommand(GameCommand.ClientTurnEnd, OnReceivedTurnEnd);
            RegisterCommand(GameCommand.ClientTrySpawnCard, OnReceivedTrySpawnCard);
            RegisterCommand(GameCommand.ClientTryAttack, OnReceivedTryAttack);

            Logic.OnDataUpdated += SendUpdateData;
            Logic.OnGameStarted += SendStartGame;
            Logic.OnManaChanged += SendChangeMana;
            Logic.OnTurnStarted += SendStartTurn;
            Logic.OnTurnEnded += SendEndTurn;
            Logic.OnCardDrawn += SendDrawCard;
            Logic.OnCardPlayed += SendPlayCardResult;
            Logic.OnCardSpawned += SendSpawnCardResult;
            Logic.OnCardAttacked += SendAttackCardResult;
            Logic.OnCardDied += SendDeadCards;
            Logic.OnGameEnded += SendEndGame;

            Managers.Instance.Network.SubscribeMessage("GameClient", OnReceivedCommand);
        }

        public void Clear()
        {
            Logic.OnDataUpdated -= SendUpdateData;
            Logic.OnGameStarted -= SendStartGame;
        }

        public void Ready(ulong clientID, List<string> deck)
        {
            Sequencer.Sequence sequence = new("Ready", Upstream);
            Sequencer.Clip clip = new("SetDeck", sequence);

            clip.OnPlay += () =>
            {
                ConnectedClients.Add(clientID);
                Logic.SetPlayerDeck(clientID, deck);

                if (ConnectedClients.Count == 2)
                {
                    Logic.StartGame();
                }
            };
        }

        private void RegisterCommand(ushort type, Action<ulong, SerializedData> callback)
        {
            Commands.Add(type, callback);
        }

        private void OnReceivedCommand(ulong clientID, FastBufferReader reader)
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

        #region OnReceive (Use queue)

        private void OnReceivedTurnEnd(ulong playerID, SerializedData sdata)
        {
            Sequencer.Sequence sequence = new("Turn end", Upstream);
            Sequencer.Clip clip = new("Turn end", sequence);

            clip.OnPlay += () =>
            {
                Logic.EndTurn();
            };
        }

        private void OnReceivedTrySpawnCard(ulong playerID, SerializedData sdata)
        {
            SerializedTrySpawnCardData spawnCard = sdata.Get<SerializedTrySpawnCardData>();

            Sequencer.Sequence sequence = new("Try spawn card", Upstream);
            Sequencer.Clip clip = new("Try spawn card", sequence);

            clip.OnPlay += () =>
            {
                Logic.TrySpawnCard(Data.GetPlayer(playerID), Data.GetHandCard(playerID, spawnCard.CardUID), spawnCard.Index);
            };
        }

        private void OnReceivedTryAttack(ulong playerID, SerializedData sdata)
        {
            SerializedTryAttackData attackData = sdata.Get<SerializedTryAttackData>();

            Sequencer.Sequence sequence = new("Try attack", Upstream);
            Sequencer.Clip clip = new("Try attack", sequence);

            clip.OnPlay += () =>
            {
                Player attackPlayer = Data.GetPlayer(attackData.AttackPlayerID);
                Card attacker = Data.GetFieldCard(attackPlayer.PlayerID, attackData.AttackerUID);
                Player defendPlayer = Data.GetPlayer(attackData.DefendPlayerID);
                Card defender = Data.GetFieldCard(defendPlayer.PlayerID, attackData.DefenderUID);

                Logic.TryAttack(attackPlayer, attacker, defendPlayer, defender);
            };
        }

        #endregion

        #region Send Utility (Not use queue, logic process queue already)

        private void SendUpdateData()
        {
            Logger.Log<GameServerEx>($"Send updated data", colorName: ColorCodes.Server);
            SerializedGameData sdata = new SerializedGameData();
            sdata.GameData = new GameData(Data);

            SendToAll(GameCommand.ServerUpdateData, sdata, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void SendStartGame()
        {
            Logger.Log<GameServerEx>($"Send start game", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerStartGame);
        }

        private void SendEndGame(ulong winnerID)
        {
            Logger.Log<GameServerEx>($"Send end game", colorName: ColorCodes.Server);
            SerializedUlong sdata = new SerializedUlong();
            sdata.value = winnerID;

            SendToAll(GameCommand.ServerEndGame, sdata, NetworkDelivery.Reliable);
        }

        private void SendChangeMana()
        {
            Logger.Log<GameServerEx>($"Send change mana", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerChangeMana);
        }

        private void SendStartTurn()
        {
            Logger.Log<GameServerEx>($"Send start turn", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerStartTurn);
        }

        private void SendEndTurn()
        {
            Logger.Log<GameServerEx>($"Send end turn", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerEndTurn);
        }

        private void SendDrawCard(ulong playerID, string cardUID)
        {
            Logger.Log<GameServerEx>($"Send draw card", colorName: ColorCodes.Server);

            SerializedDrawnCardData sdata = new SerializedDrawnCardData();
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;

            SendToAll(GameCommand.ServerDrawCard, sdata, NetworkDelivery.Reliable);
        }

        private void SendPlayCardResult(bool succeeded, ulong playerID, string cardUID)
        {
            Logger.Log<GameServerEx>($"Send play card result", colorName: ColorCodes.Server);

            SerializedPlayCardResultData sdata = new SerializedPlayCardResultData();
            sdata.Succeeded = succeeded;
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;

            SendToAll(GameCommand.ServerPlayCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendSpawnCardResult(bool succeeded, ulong playerID, string cardUID, int index)
        {
            Logger.Log<GameServerEx>($"Send spawn card result", colorName: ColorCodes.Server);

            SerializedSpawnCardResultData sdata = new();
            sdata.Succeeded = succeeded;
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;
            sdata.Index = index;

            SendToAll(GameCommand.ServerSpawnCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendAttackCardResult(bool succeeded, ulong attackPlayerID, string attackerUID, ulong defendPlayerID, string defenderUID)
        {
            Logger.Log<GameServerEx>($"Send attack card result", colorName: ColorCodes.Server);

            SerializedAttackCardResultData sdata = new();
            sdata.Succeeded = succeeded;
            sdata.AttackPlayerID = attackPlayerID;
            sdata.AttackerUID = attackerUID;
            sdata.DefendPlayerID = defendPlayerID;
            sdata.DefenderUID = defenderUID;

            SendToAll(GameCommand.ServerAttackCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendDeadCards(List<string> cards)
        {
            Logger.Log<GameServerEx>($"Send dead cards", colorName: ColorCodes.Server);

            SerializedDeadCardsData sdata = new();
            sdata.DeadCards = cards;

            SendToAll(GameCommand.ServerDeadCards, sdata, NetworkDelivery.Reliable);
        }

        #endregion

        #region Send Utilities

        private void Send(ulong target, ushort tag)
        {
            Managers.Instance.Network.SendMessage("GameServer", target, (writer) =>
            {
                writer.WriteValueSafe(tag);
            }, NetworkDelivery.ReliableSequenced);
        }

        private void Send(ulong target, ushort tag, INetworkSerializable data, NetworkDelivery delivery)
        {
            Managers.Instance.Network.SendMessage("GameServer", target, (writer) =>
            {
                writer.WriteValueSafe(tag);
                writer.WriteNetworkSerializable(data);
            }, delivery);
        }

        private void SendToAll(ushort tag)
        {
            foreach(ulong clientID in ConnectedClients)
            {
                Managers.Instance.Network.SendMessage("GameServer", clientID, (writer) =>
                {
                    writer.WriteValueSafe(tag);
                }, NetworkDelivery.ReliableSequenced);
            }
        }

        private void SendToAll(ushort tag, string data, NetworkDelivery delivery)
        {
            foreach(var clientID in ConnectedClients)
            {
                Managers.Instance.Network.SendMessage("GameServer", clientID, (writer) =>
                {
                    writer.WriteValueSafe(tag);
                    writer.WriteValueSafe(data);
                }, delivery);
            }
        }

        private void SendToAll(ushort tag, INetworkSerializable data, NetworkDelivery delivery)
        {
            foreach(var clientID in ConnectedClients)
            {
                Managers.Instance.Network.SendMessage("GameServer", clientID, (writer) =>
                {
                    writer.WriteValueSafe(tag);
                    writer.WriteNetworkSerializable(data);
                }, delivery);
            }
        }

        #endregion
    }
}