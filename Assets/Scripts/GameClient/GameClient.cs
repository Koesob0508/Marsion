using Marsion.CardView;
using Marsion.Logic;
using Marsion.Tool;
using Marsion.UI;
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
        
        [Header("Sequencer")]
        [SerializeField] private Sequencer Sequencer;

        [SerializeField] HandView hand;
        [SerializeField] FieldView playerField;
        [SerializeField] FieldView enemyField;
        [SerializeField] HeroView PlayerHero;
        [SerializeField] HeroView EnemyHero;

        public ulong ID { get; private set; }
        public ulong EnemyID { get; private set; }
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
        public event UnityAction OnManaChanged;
        public event UnityAction<bool, Player, string> OnCardPlayed;
        public event UnityAction<bool, Player, Card, int> OnCardSpawned;
        public event Action<MyTween.Sequence, Player, Card, Player, Card> OnStartAttack;
        public event Action<MyTween.Sequence> OnCharacterBeforeDead;
        public event UnityAction OnCharacterAfterDead;

        #endregion

        #region Get Operations

        public bool IsMine(Player player)
        {
            return _gameData.GetPlayer(ID) == player;
        }

        public bool IsMine(Card card)
        {
            return IsMine(card.PlayerID);
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

        public ICharacterView GetCreature(ulong clientID, string cardUID)
        {
            if (IsMine(clientID))
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

        public Card GetCard(CardType type, ulong clientID, string cardUID)
        {
            Card result = null;

            switch(type)
            {
                case CardType.Hero:
                    result = GetGameData().GetPlayer(clientID).Card;
                    break;
                case CardType.Field:
                    result = GetGameData().GetFieldCard(clientID, cardUID);
                    break;
            }

            return result;
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
            else
            {
                Managers.Logger.Log<GameClient>("Network is null", colorName: "yellow");
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

        public void TryPlayAndSpawnCard(Card card, int index)
        {
            Managers.Server.TryPlayAndSpawnCardRpc(ID, card.UID, index);
        }

        public void TurnEnd()
        {
            Managers.Server.TurnEndRpc();
        }

        public void TryAttack(Card attacker, Card defender)
        {
            Managers.Server.TryAttackRpc(attacker.PlayerID, attacker.UID, defender.PlayerID, defender.UID);
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

                // 여기에 Start와 관련한 작업이 들어가야한다.
                // 예를 들면, Player Hero와 Enemy Hero에 대해 Card를 넣는다던지.

                foreach(var player in GetGameData().Players)
                {
                    if(ID == player.ClientID)
                    {
                        PlayerHero.Init(player.Card);
                        PlayerHero.Spawn();
                    }
                    else
                    {
                        EnemyID = player.ClientID;
                        EnemyHero.Init(player.Card);
                        EnemyHero.Spawn();
                    }
                }

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

                OnDataUpdated?.Invoke();
                updateTask.OnComplete?.Invoke();
            };

            updateSequence.Append(updateTask);
            ClientSequence.Append(updateSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndGameRpc(int clientID)
        {
            MyTween.Sequence gameEndSequence = new MyTween.Sequence();
            MyTween.Task gameEndTask = new MyTween.Task();

            gameEndTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Game end.");
                UI_EndGame ui = Managers.UI.ShowPopupUI<UI_EndGame>();

                if (clientID == -1)
                {
                    ui.Text_Result.text = "DRAW";
                }
                else
                {
                    if ((ulong)clientID == ID)
                        ui.Text_Result.text = "WINNER!";
                    else
                        ui.Text_Result.text = "LOSE";
                }

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
            Managers.Logger.Log<GameClient>("Start turn", colorName: "green");
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
        public void ChangeManaRpc()
        {
            MyTween.Sequence changeManaSequence = new MyTween.Sequence();
            MyTween.Task changeManaTask = new MyTween.Task();

            changeManaTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Mana Changed");
                OnManaChanged?.Invoke();
                changeManaTask.OnComplete?.Invoke();
            };

            changeManaSequence.Append(changeManaTask);
            ClientSequence.Append(changeManaSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayCardRpc(bool succeeded, ulong clientID, string cardUID)
        {
            MyTween.Sequence cardPlaySequence = new MyTween.Sequence();
            MyTween.Task cardPlayTask = new MyTween.Task();

            cardPlayTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Card Play");
                OnCardPlayed?.Invoke(succeeded, GetGameData().GetPlayer(clientID), cardUID);
                cardPlayTask.OnComplete?.Invoke();
            };

            cardPlaySequence.Append(cardPlayTask);
            ClientSequence.Append(cardPlaySequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnCardRpc(bool succeeded, ulong clientID, string cardUID, int index)
        {
            MyTween.Sequence cardSpawnSequence = new MyTween.Sequence();
            MyTween.Task cardSpawnTask = new MyTween.Task();

            cardSpawnTask.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Card Spawn");
                OnCardSpawned?.Invoke(succeeded, GetGameData().GetPlayer(clientID), GetGameData().GetFieldCard(clientID, cardUID), index);
                cardSpawnTask.OnComplete?.Invoke();
            };

            cardSpawnSequence.Append(cardSpawnTask);
            ClientSequence.Append(cardSpawnSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void BeforeDeadCardRpc()
        {
            MyTween.Sequence beforeDeadSequence = new MyTween.Sequence();
            MyTween.Task deadLog = new MyTween.Task();

            deadLog.Action = () =>
            {
                Managers.Logger.Log<GameClient>("Card dead");
                deadLog.OnComplete?.Invoke();
            };

            beforeDeadSequence.Append(deadLog);

            MyTween.Task deadTask = new MyTween.Task();

            deadTask.Action = () =>
            {
                OnCharacterBeforeDead?.Invoke(beforeDeadSequence);
                deadTask.OnComplete?.Invoke();
            };

            beforeDeadSequence.Append(deadTask);

            ClientSequence.Append(beforeDeadSequence);
            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void AfterDeadCardRpc()
        {
            MyTween.Sequence afterDeadSequence = new MyTween.Sequence();
            MyTween.Task afterDeadTask = new MyTween.Task();

            afterDeadTask.Action = () =>
            {
                OnCharacterAfterDead?.Invoke();
                afterDeadTask.OnComplete?.Invoke();
            };

            afterDeadSequence.Append(afterDeadTask);

            ClientSequence.Append(afterDeadSequence);
            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartAttackRpc(ulong attackClientID, string attackerUID, ulong defendClientID, string defenderUID)
        {
            Player attackPlayer = GetGameData().GetPlayer(attackClientID);
            Player defendPlayer = GetGameData().GetPlayer(defendClientID);
            Card attacker = attackPlayer.GetCard(attackerUID);
            Card defender = defendPlayer.GetCard(defenderUID);

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
    }
}