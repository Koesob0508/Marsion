using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    public class DefaultGameModel : MonoBehaviour, IGameModel
    {
        [SerializeField] Sequencer Upstream;
        [SerializeField] Sequencer Downstream;

        private INetworkManagerEx _networkManager;
        private IGameLogicEx _gameLogic;
        private IGameDataHandler _dataHandler;

        private List<ulong> ConnectedClients;
        private Dictionary<ushort, Action<ulong, SerializedData>> Commands;

        public void Init(IGameModelFactory gameModelFactory)
        {
            Logger.Log<DefaultGameModel>($"Game Server initialized", colorName: ColorCodes.Server);

            ConnectedClients = new();
            Commands = new();

            _networkManager = gameModelFactory.CreateNetworkManager();
            IGameData gameData = gameModelFactory.CreateGameData();
            _dataHandler = gameModelFactory.CreateGameDataHandler(gameData);
            _gameLogic = gameModelFactory.CreateGameLogicEx(_dataHandler);

            _dataHandler.Init(); // GameData 초기화

            Upstream.Init();
            Downstream.Init();

            RegisterDefaultCommands();
            SubscribeToGameLogicEvents();

            _networkManager.SubscribeMessage("GameClient", OnReceivedCommand);
        }

        private void RegisterDefaultCommands()
        {
            RegisterCommand(GameCommand.ClientTurnEnd, OnReceivedTurnEnd);
            RegisterCommand(GameCommand.ClientTrySpawnCard, OnReceivedTrySpawnCard);
            RegisterCommand(GameCommand.ClientTryAttack, OnReceivedTryAttack);
        }

        private void SubscribeToGameLogicEvents()
        {
            _gameLogic.OnDataUpdated += SendUpdateData;
            _gameLogic.OnGameStarted += SendStartGame;
            _gameLogic.OnManaChanged += SendChangeMana;
            _gameLogic.OnTurnStarted += SendStartTurn;
            _gameLogic.OnTurnEnded += SendEndTurn;
            _gameLogic.OnCardDrawn += SendDrawCard;
            _gameLogic.OnCardPlayed += SendPlayCardResult;
            _gameLogic.OnCardSpawned += SendSpawnCardResult;
            _gameLogic.OnCardAttacked += SendAttackCardResult;
            _gameLogic.OnCardDied += SendDeadCards;
            _gameLogic.OnGameEnded += SendEndGame;
        }

        public void Clear()
        {
            _gameLogic.OnDataUpdated -= SendUpdateData;
            _gameLogic.OnGameStarted -= SendStartGame;
            _gameLogic.OnManaChanged -= SendChangeMana;
            _gameLogic.OnTurnStarted -= SendStartTurn;
            _gameLogic.OnTurnEnded -= SendEndTurn;
            _gameLogic.OnCardDrawn -= SendDrawCard;
            _gameLogic.OnCardPlayed -= SendPlayCardResult;
            _gameLogic.OnCardSpawned -= SendSpawnCardResult;
            _gameLogic.OnCardAttacked -= SendAttackCardResult;
            _gameLogic.OnCardDied -= SendDeadCards;
            _gameLogic.OnGameEnded -= SendEndGame;

            _networkManager.UnsubscribeMessage("GameClient", OnReceivedCommand);
        }

        public void Ready(ulong clientID, List<string> deck)
        {
            Sequencer.Sequence sequence = new("Ready", Upstream);
            Sequencer.Clip clip = new("SetDeck", sequence);

            clip.OnPlay += () =>
            {
                ConnectedClients.Add(clientID);
                _dataHandler.RegisterPlayerDeck(clientID, deck);

                if (ConnectedClients.Count == 2)
                {
                    _gameLogic.StartGame();
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

            if(Commands.TryGetValue(type, out var command))
            {
                command(clientID, sdata);
            }
            else
            {
                Logger.LogWarning<DefaultGameModel>($"Unknown command received: {type}");
            }
        }

        #region OnReceive (Use queue)

        private void OnReceivedTurnEnd(ulong playerID, SerializedData sdata)
        {
            Sequencer.Sequence sequence = new("Turn end", Upstream);
            Sequencer.Clip clip = new("Turn end", sequence);

            clip.OnPlay += () =>
            {
                _gameLogic.EndTurn();
            };
        }

        private void OnReceivedTrySpawnCard(ulong playerID, SerializedData sdata)
        {
            SerializedTrySpawnCardData spawnCard = sdata.Get<SerializedTrySpawnCardData>();

            Sequencer.Sequence sequence = new("Try spawn card", Upstream);
            Sequencer.Clip clip = new("Try spawn card", sequence);

            clip.OnPlay += () =>
            {
                _gameLogic.TrySpawnCard(_dataHandler.GetPlayer(playerID), _dataHandler.GetCardFromHand(playerID, spawnCard.CardUID), spawnCard.Index);
            };
        }

        private void OnReceivedTryAttack(ulong playerID, SerializedData sdata)
        {
            SerializedTryAttackData attackData = sdata.Get<SerializedTryAttackData>();

            Sequencer.Sequence sequence = new("Try attack", Upstream);
            Sequencer.Clip clip = new("Try attack", sequence);

            clip.OnPlay += () =>
            {
                Player attackPlayer = _dataHandler.GetPlayer(attackData.AttackPlayerID);
                Card attacker = _dataHandler.GetCardFromField(attackPlayer.PlayerID, attackData.AttackerUID);
                Player defendPlayer = _dataHandler.GetPlayer(attackData.DefendPlayerID);
                Card defender = _dataHandler.GetCardFromField(defendPlayer.PlayerID, attackData.DefenderUID);

                _gameLogic.TryAttack(attackPlayer, attacker, defendPlayer, defender);
            };
        }

        #endregion

        #region Send Utility (Not use queue, logic process queue already)

        private void SendUpdateData()
        {
            Logger.Log<DefaultGameModel>($"Send updated data", colorName: ColorCodes.Server);

            var sdata = new SerializedGameData();
            sdata.GameData = _dataHandler.GameData;

            SendToAll(GameCommand.ServerUpdateData, sdata, NetworkDelivery.ReliableFragmentedSequenced);
        }

        private void SendStartGame()
        {
            Logger.Log<DefaultGameModel>($"Send start game", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerStartGame);
        }

        private void SendEndGame(ulong winnerID)
        {
            Logger.Log<DefaultGameModel>($"Send end game", colorName: ColorCodes.Server);

            SerializedUlong sdata = new SerializedUlong();
            sdata.value = winnerID;

            SendToAll(GameCommand.ServerEndGame, sdata);
        }

        private void SendChangeMana()
        {
            Logger.Log<DefaultGameModel>($"Send change mana", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerChangeMana);
        }

        private void SendStartTurn()
        {
            Logger.Log<DefaultGameModel>($"Send start turn", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerStartTurn);
        }

        private void SendEndTurn()
        {
            Logger.Log<DefaultGameModel>($"Send end turn", colorName: ColorCodes.Server);

            SendToAll(GameCommand.ServerEndTurn);
        }

        private void SendDrawCard(ulong playerID, string cardUID)
        {
            Logger.Log<DefaultGameModel>($"Send draw card", colorName: ColorCodes.Server);

            SerializedDrawnCardData sdata = new SerializedDrawnCardData();
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;

            SendToAll(GameCommand.ServerDrawCard, sdata, NetworkDelivery.Reliable);
        }

        private void SendPlayCardResult(bool succeeded, ulong playerID, string cardUID)
        {
            Logger.Log<DefaultGameModel>($"Send play card result", colorName: ColorCodes.Server);

            SerializedPlayCardResultData sdata = new SerializedPlayCardResultData();
            sdata.Succeeded = succeeded;
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;

            SendToAll(GameCommand.ServerPlayCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendSpawnCardResult(bool succeeded, ulong playerID, string cardUID, int index)
        {
            Logger.Log<DefaultGameModel>($"Send spawn card result", colorName: ColorCodes.Server);

            SerializedSpawnCardResultData sdata = new();
            sdata.Succeeded = succeeded;
            sdata.PlayerID = playerID;
            sdata.CardUID = cardUID;
            sdata.Index = index;

            SendToAll(GameCommand.ServerSpawnCardResult, sdata, NetworkDelivery.Reliable);
        }

        private void SendAttackCardResult(bool succeeded, ulong attackPlayerID, string attackerUID, ulong defendPlayerID, string defenderUID)
        {
            Logger.Log<DefaultGameModel>($"Send attack card result", colorName: ColorCodes.Server);

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
            Logger.Log<DefaultGameModel>($"Send dead cards", colorName: ColorCodes.Server);

            SerializedDeadCardsData sdata = new();
            sdata.DeadCards = cards;

            SendToAll(GameCommand.ServerDeadCards, sdata, NetworkDelivery.Reliable);
        }

        #endregion

       #region Send Utilities

        private void SendToAll(ushort tag, INetworkSerializable data = null, NetworkDelivery delivery = NetworkDelivery.ReliableSequenced)
        {
            foreach(ulong clientID in ConnectedClients)
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