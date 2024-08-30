using Marsion.CardView;
using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


namespace Marsion.Client
{
    public class GameClient : NetworkBehaviour, IGameClient
    {
        #region Resource Fields

        public List<Sprite> PortraitSprites;

        #endregion

        GameData _gameData;
        MyTween.MainSequence ClientSequence;

        [SerializeField] HandView hand;
        [SerializeField] FieldView playerField;
        [SerializeField] FieldView enemyField;

        public ulong ID { get; private set; }
        public IHandView Hand { get => hand; }
        public IFieldView PlayerField { get => playerField; }
        public IFieldView EnemyField { get => enemyField; }

        public InputManager Input { get; private set; }

        #region Events

        public event UnityAction OnDataUpdated;
        public event UnityAction OnGameStarted;
        public event UnityAction OnGameEnded;
        public event UnityAction OnTurnStarted;
        public event UnityAction OnTurnEnded;
        public event UnityAction<Player, Card> OnCardDrawn;
        public event UnityAction<Player, string> OnCardPlayed;
        public event UnityAction<Player, Card, int> OnCardSpawned;
        public event Action<MyTween.Sequence, Player, Card, Player, Card> OnStartAttack;
        public event Action<MyTween.Sequence> OnCreatureDead;

        #endregion

        #region Get Operations

        public bool IsMine(Player player)
        {
            return _gameData.GetPlayer(ID) == player;
        }

        public bool IsMine(Card card)
        {
            return IsMine(card.ClientID);
        }

        public bool IsMine(ulong id)
        {
            return ID == id;
        }

        public bool IsMyTurn()
        {
            return IsMine(GetGameData().CurrentPlayer);
        }

        public GameData GetGameData()
        {
            return _gameData;
        }

        public ICreatureView GetCreature(ulong clientID, string cardUID)
        {
            if(IsMine(clientID))
            {
                return PlayerField.GetCreature(GetGameData().GetFieldCard(clientID, cardUID));
            }
            else
            {
                return EnemyField.GetCreature(GetGameData().GetFieldCard(clientID, cardUID));
            }
        }

        public Sprite GetPortrait(int index)
        {
            return PortraitSprites[index];
        }

        #endregion

        #region Manager Operations

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= SetClientID;
                Managers.Network.OnClientConnectedCallback += SetClientID;
            }

            ClientSequence = new MyTween.MainSequence();

            Input = new InputManager();
        }

        public void Update()
        {
            Input.Update();
        }

        public void Clear()
        {
            Managers.Network.OnClientConnectedCallback -= SetClientID;
        }

        #endregion

        #region Client Operations

        public void SetClientID(ulong clientID)
        {
            ID = Managers.Network.LocalClientId;
        }

        public void DrawCard()
        {
            Managers.Server.DrawCardRpc(ID);
        }

        public void PlayCard(Card card)
        {
            Managers.Server.PlayCardRpc(ID, card.UID);
        }

        public void PlayAndSpawnCard(Card card, int index)
        {
            Managers.Server.PlayAndSpawnCardRpc(ID, card.UID, index);
        }

        public void TurnEnd()
        {
            Managers.Server.TurnEndRpc();
        }
        
        public void TryAttack(Card attacker, Card defender)
        {
            Managers.Server.TryAttackRpc(attacker.ClientID, attacker.UID, defender.ClientID, defender.UID);
        }

        #endregion

        #region Event Rpcs

        [Rpc(SendTo.ClientsAndHost)]
        public void StartGameRpc()
        {
            MyTween.Sequence gameStartSequence = new MyTween.Sequence();
            MyTween.Task gameStartTask = new MyTween.Task();

            gameStartTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Game Start");
                OnGameStarted?.Invoke();
                gameStartTask.OnComplete?.Invoke();
            };

            gameStartSequence.Append(gameStartTask);
            ClientSequence.Append(gameStartSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void UpdateDataRpc(NetworkGameData networkData)
        {
            MyTween.Sequence updateSequence = new MyTween.Sequence();
            MyTween.Task updateTask = new MyTween.Task();

            updateTask.Action = () =>
            {
                _gameData = networkData.gameData;
                Managers.Logger.Log<GameClient>("Game data updated");

                foreach (Player player in _gameData.Players)
                {
                    foreach (Card card in player.Field)
                    {
                        if (card.HP <= 0)
                            card.Die();

                        Managers.Logger.Log<GameClient>($"{_gameData.GetFieldCard(player.ClientID, card.UID).IsDead}", colorName: "blue");
                    }
                }

                OnDataUpdated?.Invoke();
                updateTask.OnComplete?.Invoke();
            };

            updateSequence.Append(updateTask);
            ClientSequence.Append(updateSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndGameRpc()
        {
            MyTween.Sequence gameEndSequence = new MyTween.Sequence();
            MyTween.Task gameEndTask = new MyTween.Task();

            gameEndTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Game end.");
                OnGameEnded?.Invoke();
                gameEndTask.OnComplete?.Invoke();
            };

            gameEndSequence.Append(gameEndTask);
            ClientSequence.Append(gameEndSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartTurnRpc()
        {
            MyTween.Sequence turnStartSequence = new MyTween.Sequence();
            MyTween.Task turnStartTask = new MyTween.Task();

            turnStartTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Turn Start.");
                OnTurnStarted?.Invoke();
                turnStartTask.OnComplete?.Invoke();
            };

            turnStartSequence.Append(turnStartTask);
            ClientSequence.Append(turnStartSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndTurnRpc()
        {
            MyTween.Sequence turnEndSequence = new MyTween.Sequence();
            MyTween.Task turnEndTask = new MyTween.Task();

            turnEndTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Turn End.");
                OnTurnEnded?.Invoke();
                turnEndTask.OnComplete?.Invoke();
            };

            turnEndSequence.Append(turnEndTask);
            ClientSequence.Append(turnEndSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DrawCardRpc(ulong clientID, string cardUID)
        {
            MyTween.Sequence cardDrawSequence = new MyTween.Sequence();
            MyTween.Task cardDrawTask = new MyTween.Task();

            cardDrawTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Card Draw");
                OnCardDrawn?.Invoke(GetGameData().GetPlayer(clientID), GetGameData().GetHandCard(clientID, cardUID));
                cardDrawTask.OnComplete?.Invoke();
            };

            cardDrawSequence.Append(cardDrawTask);
            ClientSequence.Append(cardDrawSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayCardRpc(ulong clientID, string cardUID)
        {
            MyTween.Sequence cardPlaySequence = new MyTween.Sequence();
            MyTween.Task cardPlayTask = new MyTween.Task();

            cardPlayTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Card Play");
                OnCardPlayed?.Invoke(GetGameData().GetPlayer(clientID), cardUID);
                cardPlayTask.OnComplete?.Invoke();
            };

            cardPlaySequence.Append(cardPlayTask);
            ClientSequence.Append(cardPlaySequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnCardRpc(ulong clientID, string cardUID, int index)
        {
            MyTween.Sequence cardSpawnSequence = new MyTween.Sequence();
            MyTween.Task cardSpawnTask = new MyTween.Task();

            cardSpawnTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Card Spawn");
                OnCardSpawned?.Invoke(GetGameData().GetPlayer(clientID), GetGameData().GetFieldCard(clientID, cardUID), index);
                cardSpawnTask.OnComplete?.Invoke();
            };

            cardSpawnSequence.Append(cardSpawnTask);
            ClientSequence.Append(cardSpawnSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DeadCardRpc()
        {
            MyTween.Sequence deadSequence = new MyTween.Sequence();
            MyTween.Task deadLog = new MyTween.Task();

            deadLog.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Card dead");
                deadLog.OnComplete?.Invoke();
            };

            deadSequence.Append(deadLog);

            OnCreatureDead?.Invoke(deadSequence);

            ClientSequence.Append(deadSequence);
            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartAttackRpc(ulong attackClientID, string attackerUID, ulong defendClientID, string defenderUID)
        {
            Player attackPlayer = GetGameData().GetPlayer(attackClientID);
            Player defendPlayer = GetGameData().GetPlayer(defendClientID);
            Card attacker = GetGameData().GetFieldCard(attackClientID, attackerUID);
            Card defender = GetGameData().GetFieldCard(defendClientID, defenderUID);

            MyTween.Sequence attackSequence = new MyTween.Sequence();
            MyTween.Task attackLog = new MyTween.Task();

            attackLog.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Start Attack");
                attackLog.OnComplete?.Invoke();
            };

            attackSequence.Append(attackLog);

            OnStartAttack?.Invoke(attackSequence, attackPlayer, attacker, defendPlayer, defender);

            ClientSequence.Append(attackSequence);
            ClientSequence.Play();
        }

        #endregion

        public bool TryPlayCard(Card card)
        {
            return true;
        }

        
    }
}