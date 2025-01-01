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
        private Dictionary<ushort, Action<ulong, SerializedData>> Commands;

        public void Init(IGameSessionFactory sessionFactory)
        {
            Logger.Log<DefaultGameSession>($"Game Server initialized", colorName: ColorCodes.Server);

            Commands = new();

            _ClientPlayerIdMap = sessionFactory.ProvidePlayersClientIDs();
            _networkManager = sessionFactory.ProvideNetwork();
            _gameLogic = sessionFactory.CreateGameLogic();
            _gameLogic.Init(sessionFactory.CreateGameLogicFactory());

            Upstream.Init();

            RegisterDefaultCommands();
            SubscribeToGameLogicEvents();

            _networkManager.SubscribeMessage("GameClient", OnReceivedCommand);

            _gameLogic.StartGame();
        }

        private void RegisterDefaultCommands()
        {
            RegisterCommand(GameMessageCode.ClientTurnEnd, OnReceivedTurnEnd);
            RegisterCommand(GameMessageCode.ClientTrySpawnCard, OnReceivedTrySpawnCard);
            RegisterCommand(GameMessageCode.ClientTryAttack, OnReceivedTryAttack);
        }

        private void SubscribeToGameLogicEvents()
        {
            _gameLogic.SendDataUpdated += SendUpdateData;
            _gameLogic.SendGameStarted += SendStartGame;
            _gameLogic.SendManaChanged += SendChangeMana;
            _gameLogic.SendTurnStarted += SendStartTurn;
            _gameLogic.SendTurnEnded += SendEndTurn;
            _gameLogic.SendCardDrawn += SendDrawCard;
            _gameLogic.SendCardPlayed += SendPlayCardResult;
            _gameLogic.SendCardSpawned += SendSpawnCardResult;
            _gameLogic.SendCardAttacked += SendAttackCardResult;
            _gameLogic.SendCardDied += SendDeadCards;
            _gameLogic.SendGameEnded += SendEndGame;
        }

        public void Clear()
        {
            _gameLogic.SendDataUpdated -= SendUpdateData;
            _gameLogic.SendGameStarted -= SendStartGame;
            _gameLogic.SendManaChanged -= SendChangeMana;
            _gameLogic.SendTurnStarted -= SendStartTurn;
            _gameLogic.SendTurnEnded -= SendEndTurn;
            _gameLogic.SendCardDrawn -= SendDrawCard;
            _gameLogic.SendCardPlayed -= SendPlayCardResult;
            _gameLogic.SendCardSpawned -= SendSpawnCardResult;
            _gameLogic.SendCardAttacked -= SendAttackCardResult;
            _gameLogic.SendCardDied -= SendDeadCards;
            _gameLogic.SendGameEnded -= SendEndGame;

            _networkManager.UnsubscribeMessage("GameClient", OnReceivedCommand);
        }

        private void RegisterCommand(ushort type, Action<ulong, SerializedData> callback)
        {
            Commands.Add(type, callback);
        }

        private void OnReceivedCommand(ulong clientID, FastBufferReader reader)
        {
            reader.ReadValueSafe(out ushort type);
            SerializedData sdata = new SerializedData(reader);

            if(Commands.TryGetValue(type, out var command))
            {
                command(clientID, sdata);
            }
            else
            {
                Logger.LogWarning<DefaultGameSession>($"Unknown command received: {type}");
            }
        }

        #region OnReceive (Use queue)

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

        private void SendUpdateData(IGameData gameData)
        {
            Logger.Log<DefaultGameSession>($"Send updated data", colorName: ColorCodes.Server);

            var sdata = new SerializedGameData();
            sdata.GameData = gameData;

            SendToAll(GameMessageCode.ServerUpdateData, sdata, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void SendStartGame()
        {
            Logger.Log<DefaultGameSession>($"Send start game", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerStartGame);
        }

        private void SendEndGame(ulong winnerID)
        {
            Logger.Log<DefaultGameSession>($"Send end game", colorName: ColorCodes.Server);

            SerializedUlong sdata = new SerializedUlong();
            sdata.value = winnerID;

            SendToAll(GameMessageCode.ServerEndGame, sdata);
        }

        private void SendChangeMana()
        {
            Logger.Log<DefaultGameSession>($"Send change mana", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerChangeMana);
        }

        private void SendStartTurn()
        {
            Logger.Log<DefaultGameSession>($"Send start turn", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerStartTurn);
        }

        private void SendEndTurn()
        {
            Logger.Log<DefaultGameSession>($"Send end turn", colorName: ColorCodes.Server);

            SendToAll(GameMessageCode.ServerEndTurn);
        }

        private void SendDrawCard(ulong playerID, string cardUID)
        {
            Logger.Log<DefaultGameSession>($"Send draw card", colorName: ColorCodes.Server);

            SerializedDrawnCardData sdata = new SerializedDrawnCardData();
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;

            SendToAll(GameMessageCode.ServerDrawCard, sdata, NetworkDelivery.Reliable);
        }

        private void SendPlayCardResult(bool succeeded, ulong playerID, string cardUID)
        {
            Logger.Log<DefaultGameSession>($"Send play card result", colorName: ColorCodes.Server);

            SerializedPlayCardResultData sdata = new SerializedPlayCardResultData();
            sdata.Succeeded = succeeded;
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;

            SendToAll(GameMessageCode.ServerPlayCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendSpawnCardResult(bool succeeded, ulong playerID, string cardUID, int index)
        {
            Logger.Log<DefaultGameSession>($"Send spawn card result", colorName: ColorCodes.Server);

            SerializedSpawnCardResultData sdata = new();
            sdata.Succeeded = succeeded;
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;
            sdata.Index = index;

            SendToAll(GameMessageCode.ServerSpawnCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendAttackCardResult(bool succeeded, ulong attackPlayerID, string attackerUID, ulong defendPlayerID, string defenderUID)
        {
            Logger.Log<DefaultGameSession>($"Send attack card result", colorName: ColorCodes.Server);

            SerializedAttackCardResultData sdata = new();
            sdata.Succeeded = succeeded;
            sdata.AttackPlayerID = attackPlayerID;
            sdata.AttackerUID = attackerUID;
            sdata.DefendPlayerID = defendPlayerID;
            sdata.DefenderUID = defenderUID;

            SendToAll(GameMessageCode.ServerAttackCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendDeadCards(List<string> cards)
        {
            Logger.Log<DefaultGameSession>($"Send dead cards", colorName: ColorCodes.Server);

            SerializedDeadCardsData sdata = new();
            sdata.DeadCards = cards;

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