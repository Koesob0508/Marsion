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
        public List<Sprite> PortraitSprites;

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

        public event UnityAction OnDataUpdated;
        public event UnityAction OnGameStarted;
        public event UnityAction OnGameEnded;
        public event UnityAction OnTurnStarted;
        public event UnityAction OnTurnEnded;
        public event UnityAction<Player, Card> OnCardDrawn;
        public event UnityAction OnManaChanged;

        public event UnityAction<bool, Player, string> OnCardPlayed;
        public event UnityAction<bool, Player, Card, int> OnCardSpawned;
        public event Action<Sequence, Player, Card, Player, Card> OnStartAttack;
        public event Action<Sequence> OnCharacterBeforeDead;
        public event UnityAction OnCharacterAfterDead;

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

            ClientSequence = new Sequencer();

            Input = new InputManager();

            Managers.Server.OnDataUpdated += UpdateDataRpc;
            Managers.Server.OnGameStarted += StartGameRpc;
            Managers.Server.OnGameEnded += EndGameRpc;
            Managers.Server.OnTurnStarted += StartTurnRpc;
            Managers.Server.OnTurnEnded += EndTurnRpc;
            Managers.Server.OnCardDrawn += DrawCardRpc;
            Managers.Server.OnManaChanged += ChangeManaRpc;
            Managers.Server.OnCardPlayed += PlayCardRpc;
            Managers.Server.OnCardSpawned += SpawnCardRpc;
            Managers.Server.OnStartAttack += StartAttackRpc;
            Managers.Server.OnBeforeCardDead += BeforeDeadCardRpc;
            Managers.Server.OnAfterCardDead += AfterDeadCardRpc;
        }

        public void Update()
        {
            Input.Update();
        }

        public void Clear()
        {
            Managers.Network.OnClientConnectedCallback -= SetClientID;
        }

        public void SetClientID(ulong clientID)
        {
            ID = Managers.Network.LocalClientId;
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

        [Rpc(SendTo.ClientsAndHost)]
        public void StartGameRpc()
        {
            Sequence gameStartSequence = new Sequence();
            Clip gameStartTask = new Clip("GameStart");

            gameStartTask.Action += () =>
            {
                Managers.Logger.Log<GameClient>("Start game", colorName: "green");

                foreach (var player in GetGameData().Players)
                {
                    if (ID == player.ClientID)
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
            };

            gameStartSequence.Append(gameStartTask);
            ClientSequence.Append(gameStartSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void UpdateDataRpc(NetworkGameData networkData)
        {
            Sequence updateSequence = new Sequence();
            Clip updateTask = new Clip("UpdateClip");

            updateTask.Action += () =>
            {
                _gameData = networkData.gameData;
                Managers.Logger.Log<GameClient>("Game data updated", colorName:"green");

                OnDataUpdated?.Invoke();
            };

            updateSequence.Append(updateTask);
            ClientSequence.Append(updateSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndGameRpc(int clientID)
        {
            Sequence gameEndSequence = new Sequence();
            Clip gameEndTask = new Clip("GameEnd");

            gameEndTask.Action += () =>
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
            };


            gameEndSequence.Append(gameEndTask);
            ClientSequence.Append(gameEndSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartTurnRpc()
        {
            Sequence turnStartSequence = new Sequence();
            Clip turnStartTask = new Clip("TurnStart");

            turnStartTask.Action += () =>
            {
                Managers.Logger.Log<GameClient>("Turn start", colorName: "green");
                OnTurnStarted?.Invoke();
            };

            turnStartSequence.Append(turnStartTask);
            ClientSequence.Append(turnStartSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndTurnRpc()
        {
            Sequence turnEndSequence = new Sequence();
            Clip turnEndTask = new Clip("TurnEnd");

            turnEndTask.Action += () =>
            {
                Managers.Logger.Log<GameClient>("Turn end", colorName: "green");
                OnTurnEnded?.Invoke();
            };

            turnEndSequence.Append(turnEndTask);
            ClientSequence.Append(turnEndSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DrawCardRpc(ulong clientID, string cardUID)
        {
            Sequence cardDrawSequence = new Sequence();
            Clip cardDrawTask = new Clip("CardDraw");

            cardDrawTask.Action += () =>
            {
                Managers.Logger.Log<GameClient>("Card draw", colorName: "green");
                OnCardDrawn?.Invoke(GetGameData().GetPlayer(clientID), GetGameData().GetHandCard(clientID, cardUID));
            };

            cardDrawSequence.Append(cardDrawTask);
            ClientSequence.Append(cardDrawSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ChangeManaRpc()
        {
            Sequence changeManaSequence = new Sequence();
            Clip changeManaTask = new Clip("ChangeMana");

            changeManaTask.Action += () =>
            {
                Managers.Logger.Log<GameClient>("Mana Changed", colorName: "green");
                OnManaChanged?.Invoke();
            };

            changeManaSequence.Append(changeManaTask);
            ClientSequence.Append(changeManaSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayCardRpc(bool succeeded, ulong clientID, string cardUID)
        {
            Sequence cardPlaySequence = new Sequence();
            Clip cardPlayTask = new Clip("CardPlay", autoComplete: true);

            cardPlayTask.Action += () =>
            {
                Managers.Logger.Log<GameClient>($"Card play succeeded? : {succeeded}", colorName: "green");
                OnCardPlayed?.Invoke(succeeded, GetGameData().GetPlayer(clientID), cardUID);
            };

            cardPlaySequence.Append(cardPlayTask);
            ClientSequence.Append(cardPlaySequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnCardRpc(bool succeeded, ulong clientID, string cardUID, int index)
        {
            Sequence cardSpawnSequence = new Sequence();
            Clip cardSpawnTask = new Clip("CardSpawn");

            cardSpawnTask.Action += () =>
            {
                Managers.Logger.Log<GameClient>($"Card spawn succeeded? : {succeeded}", colorName: "green");
                OnCardSpawned?.Invoke(succeeded, GetGameData().GetPlayer(clientID), GetGameData().GetFieldCard(clientID, cardUID), index);
            };

            cardSpawnSequence.Append(cardSpawnTask);
            ClientSequence.Append(cardSpawnSequence);

            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void BeforeDeadCardRpc()
        {
            Sequence beforeDeadSequence = new Sequence();
            Clip deadLog = new Clip("DeadLog");

            deadLog.Action += () =>
            {
                Managers.Logger.Log<GameClient>("Card dead");
            };

            beforeDeadSequence.Append(deadLog);

            Clip deadTask = new Clip("Dead");

            deadTask.Action += () =>
            {
                OnCharacterBeforeDead?.Invoke(beforeDeadSequence);
            };

            beforeDeadSequence.Append(deadTask);

            ClientSequence.Append(beforeDeadSequence);
            ClientSequence.Play();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void AfterDeadCardRpc()
        {
            Sequence afterDeadSequence = new Sequence();
            Clip afterDeadTask = new Clip("AfterDead");

            afterDeadTask.Action += () =>
            {
                OnCharacterAfterDead?.Invoke();
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

            Sequence attackSequence = new Sequence();
            Clip attackLog = new Clip("AttackLog");

            attackLog.Action += () =>
            {
                Managers.Logger.Log<GameClient>("Start Attack");
            };

            attackSequence.Append(attackLog);

            OnStartAttack?.Invoke(attackSequence, attackPlayer, attacker, defendPlayer, defender);

            ClientSequence.Append(attackSequence);
            ClientSequence.Play();
        }

        #region Utils

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

            switch (type)
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
    }
}