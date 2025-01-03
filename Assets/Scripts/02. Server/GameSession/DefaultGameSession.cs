using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class DefaultGameSession : MonoBehaviour, IGameSession
    {
        [SerializeField] Sequencer Upstream;

        private INetworkManagerEx _networkManager;
        private IGameLogic _gameLogic;

        private Dictionary<ulong, ushort> _ClientPlayerIdMap;
        private Dictionary<ushort, Action<ulong, SerializedData>> Messages;
        private Dictionary<string, Action<GameCommandData>> Events;

        public void Init(IGameSessionFactory sessionFactory)
        {
            Logger.Log<DefaultGameSession>($"Game Server initialized", colorName: ColorCodes.Server);

            Messages = new();

            _ClientPlayerIdMap = sessionFactory.ProvidePlayersClientIDs();
            _networkManager = sessionFactory.ProvideNetwork();
            _gameLogic = sessionFactory.CreateGameLogic();
            _gameLogic.Init(sessionFactory.CreateGameLogicFactory());

            Upstream.Init();

            RegisterMessages();
            RegisterEvents();

            _networkManager.SubscribeMessage("GameClient", OnReceivedClientMessage);
            _gameLogic.SubscribeEvent(OnReceivedSendRequest);

            _gameLogic.StartGame();
        }

        private void RegisterMessages()
        {
            RegisterMessage(GameMessageCode.ClientTurnEnd, OnReceivedTurnEnd);
            RegisterMessage(GameMessageCode.ClientTrySpawnCard, OnReceivedTrySpawnCard);
            RegisterMessage(GameMessageCode.ClientTryAttack, OnReceivedTryAttack);
        }

        private void RegisterEvents()
        {
            RegisterEvent("UpdateData", SendUpdateData);
            RegisterEvent("StartGame", SendStartGame);
            RegisterEvent("EndGame", SendEndGame);
            RegisterEvent("StartTurn", SendStartTurn);
            RegisterEvent("EndTurn", SendEndTurn);
            RegisterEvent("DrawCard", SendDrawCard);
            RegisterEvent("ChangeMana", SendChangeMana);
            RegisterEvent("FailedPlayCard", SendFailedPlay);
            RegisterEvent("PlayCard", SendPlayCard);
            RegisterEvent("SpawnCard", SendSpawnCard);
            RegisterEvent("AttackCard", SendAttackCard);
            RegisterEvent("DieCards", SendDieCards);
        }

        public void Clear()
        {
            _networkManager.UnsubscribeMessage("GameClient", OnReceivedClientMessage);
            _gameLogic.UnsubscribeEvent(OnReceivedSendRequest);
        }

        private void RegisterMessage(ushort type, Action<ulong, SerializedData> callback)
        {
            Messages.Add(type, callback);
        }

        private void OnReceivedClientMessage(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort type);
            SerializedData sdata = new SerializedData(reader);

            if(Messages.TryGetValue(type, out var onReceivedMessage))
            {
                onReceivedMessage(clientID, sdata);
            }
            else
            {
                Logger.LogWarning<DefaultGameSession>($"Unknown command received: {type}");
            }
        }

        private void RegisterEvent(string eventName, Action<GameCommandData> listener)
        {
            Events.Add(eventName, listener);
        }

        private void OnReceivedSendRequest(string eventName, GameCommandData cdata)
        {
            if(Events.TryGetValue(eventName, out var sendRequest))
            {
                sendRequest(cdata);
            }
            else
            {
                Logger.LogWarning<DefaultGameSession>($"Unknown event name received: {eventName}");
            }
        }

        #region OnReceiveClientMessage (Use queue)

        private void OnReceivedTurnEnd(ulong clientID, SerializedData sdata)
        {
            Sequencer.Sequence sequence = new("Turn end", Upstream);
            Sequencer.Clip clip = new("Turn end", sequence);

            clip.OnPlay += () =>
            {
                _gameLogic.EndTurn();
            };
        }

        private void OnReceivedTrySpawnCard(ulong clientID, SerializedData sdata)
        {
            SerializedTrySpawnCardData spawnCard = sdata.Get<SerializedTrySpawnCardData>();

            Sequencer.Sequence sequence = new("Try spawn card", Upstream);
            Sequencer.Clip clip = new("Try spawn card", sequence);

            clip.OnPlay += () =>
            {
                _ClientPlayerIdMap.TryGetValue(clientID, out var playerID);
                _gameLogic.TrySpawnCard(playerID, spawnCard.CardUID, spawnCard.Index);
            };
        }

        private void OnReceivedTryAttack(ulong clientID, SerializedData sdata)
        {
            SerializedTryAttackData attackData = sdata.Get<SerializedTryAttackData>();

            Sequencer.Sequence sequence = new("Try attack", Upstream);
            Sequencer.Clip clip = new("Try attack", sequence);

            clip.OnPlay += () =>
            {
                _gameLogic.TryAttack(attackData.AttackPlayerID, attackData.AttackerUID, attackData.DefendPlayerID, attackData.DefenderUID);
            };
        }

        #endregion

        #region Send Utility (Not use queue, logic process queue already)

        private void SendUpdateData(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send updated data", colorName: ColorCodes.Server);

            var sdata = new SerializedGameData();
            sdata.GameData = _gameLogic.GameData;

            SendToAll(GameMessageCode.ServerUpdateData, sdata, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void SendStartGame(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send start game", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerStartGame);
        }

        private void SendEndGame(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send end game", colorName: ColorCodes.Server);

            SerializedUlong sdata = new SerializedUlong();
            sdata.value = cdata.PlayerID;

            SendToAll(GameMessageCode.ServerEndGame, sdata);
        }

        private void SendChangeMana(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send change mana", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerChangeMana);
        }

        private void SendStartTurn(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send start turn", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerStartTurn);
        }

        private void SendEndTurn(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send end turn", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerEndTurn);
        }

        private void SendDrawCard(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send draw card", colorName: ColorCodes.Server);

            SerializedDrawnCardData sdata = new SerializedDrawnCardData();
            sdata.PlayerID = cdata.PlayerID;
            sdata.CardUID = cdata.CardUID;

            SendToAll(GameMessageCode.ServerDrawCard, sdata, NetworkDelivery.Reliable);
        }

        private void SendPlayCard(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send play card result", colorName: ColorCodes.Server);

            SerializedPlayCardResultData sdata = new();
            sdata.Succeeded = cdata.Succeeded;
            sdata.PlayerID = cdata.PlayerID;
            sdata.CardUID = cdata.CardUID;

            SendToAll(GameMessageCode.ServerPlayCardResult, sdata, NetworkDelivery.ReliableSequenced);
        }

        private void SendFailedPlay(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send failed play card", colorName: ColorCodes.Server);

            SerializedSpawnCardResultData sdata = new();
            sdata.Succeeded = false;
            sdata.PlayerID = cdata.PlayerID;
            sdata.CardUID = cdata.CardUID;
            sdata.Index = cdata.IntValue;

            SendToAll(GameMessageCode.ServerSpawnCardResult, sdata, NetworkDelivery.ReliableSequenced);
        }

        private void SendSpawnCard(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send spawn card result", colorName: ColorCodes.Server);

            SerializedSpawnCardResultData sdata = new();
            sdata.Succeeded = true;
            sdata.PlayerID = cdata.PlayerID;
            sdata.CardUID = cdata.CardUID;
            sdata.Index = cdata.IntValue;

            SendToAll(GameMessageCode.ServerSpawnCardResult, sdata, NetworkDelivery.ReliableSequenced);
        }

        private void SendAttackCard(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send attack card result", colorName: ColorCodes.Server);

            SerializedAttackCardResultData sdata = new();
            sdata.Succeeded = cdata.Succeeded;
            sdata.AttackPlayerID = cdata.PlayerID;
            sdata.AttackerUID = cdata.CardUID;
            sdata.DefendPlayerID = cdata.TargetPlayerID_2;
            sdata.DefenderUID = cdata.TargetCardUID_2;

            SendToAll(GameMessageCode.ServerAttackCardResult, sdata, NetworkDelivery.Reliable);

        }

        private void SendDieCards(GameCommandData cdata)
        {
            Logger.Log<DefaultGameSession>($"Send dead cards", colorName: ColorCodes.Server);

            SerializedDeadCardsData sdata = new();
            sdata.DeadCards = cdata.CardUIDs;

            SendToAll(GameMessageCode.ServerDeadCards, sdata, NetworkDelivery.Reliable);
        }

        #endregion

       #region Send Utilities

        private void SendToAll(ushort tag, INetworkSerializable data = null, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
        {
            foreach(ulong clientID in _ClientPlayerIdMap.Keys)
            {
                _networkManager.SendMessage("GameServer", clientID, (writer) =>
                {
                    writer.WriteValueSafe(tag);
                    if (data != null)
                    {
                        writer.WriteNetworkSerializable(data);
                    }
                }, delivery);
            }
        }

        #endregion
    }
}