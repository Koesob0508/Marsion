using Marsion.CardView;
using Marsion.Logic;
using Marsion.Tool;
using Marsion.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


namespace Marsion.Client
{
    public class GameClient : NetworkBehaviour, IGameClient
    {
        public List<Sprite> PortraitSprites;

        GameData _gameData;

        [Header("Sequencer")]
        [SerializeField] Sequencer Sequencer;

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

        public event Action OnSuccessRelay;
        public event Action OnDeckBuildingUpdated;
        public event UnityAction OnDataUpdated;
        public event UnityAction OnGameStarted;
        public event UnityAction OnGameEnded;
        public event Action OnGameReset;
        public event UnityAction OnTurnStarted;
        public event UnityAction OnTurnEnded;
        public event UnityAction<Player, Card> OnCardDrawn;
        public event UnityAction OnManaChanged;

        public event UnityAction<bool, Player, string> OnCardPlayed;
        public event UnityAction<bool, Player, Card, int> OnCardSpawned;
        public event Action<Sequencer.Sequence, Player, Card, Player, Card> OnStartAttack;

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientJoin -= SetClientID;
                Managers.Network.OnClientJoin += SetClientID;
            }
            else
            {
                Managers.Logger.Log<GameClient>("Network is null", colorName: "yellow");
            }

            Input = new InputManager();
            Sequencer.Init();

            Managers.Server.OnStartDeckBuilding += StartDeckBuildingRpc;
            Managers.Server.OnUpdateDeckBuildingState += UpdateDeckBuildingRpc;

            Managers.Server.OnDataUpdated += UpdateDataRpc;
            Managers.Server.OnGameStarted += StartGameRpc;
            Managers.Server.OnGameEnded += EndGameRpc;
            Managers.Server.OnResetGame += ResetGameRpc;
            Managers.Server.OnTurnStarted += StartTurnRpc;
            Managers.Server.OnTurnEnded += EndTurnRpc;
            Managers.Server.OnCardDrawn += DrawCardRpc;
            Managers.Server.OnManaChanged += ChangeManaRpc;
            Managers.Server.OnCardPlayed += PlayCardRpc;
            Managers.Server.OnCardSpawned += SpawnCardRpc;
            Managers.Server.OnStartAttack += StartAttackRpc;
            Managers.Server.OnDeadCard += DeadCardRpc;
        }

        private void Update()
        {
            Input.Update();
        }

        public void Clear()
        {
            Managers.Network.OnClientJoin -= SetClientID;
        }

        public void SetClientID(ulong clientID)
        {
            ID = Managers.Network.ClientID;
        }

        public void Ready(List<Card> deckSO)
        {
            List<SerializedCardData> deck = new List<SerializedCardData>();

            foreach (var card in deckSO)
            {
                SerializedCardData netCard = new SerializedCardData();
                netCard.UID = card.UID;
                deck.Add(netCard);
            }

            SerializedCardData[] netDeck = deck.ToArray();
            Managers.Server.ReadyRpc(netDeck);
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
        private void StartDeckBuildingRpc()
        {
            Managers.Logger.Log<GameClient>("StartBuilding", colorName: "green");

            OnSuccessRelay?.Invoke();
            Managers.UI.ShowPopupUI<UI_DeckBuilder>();
        }

        [Rpc(SendTo.ClientsAndHost)]
        private void UpdateDeckBuildingRpc(ulong clientID)
        {
            if (clientID == ID)
            {
                OnDeckBuildingUpdated?.Invoke();
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartGameRpc()
        {
            Sequencer.Sequence gameStartSequence = new Sequencer.Sequence("GameStart", Sequencer);
            Sequencer.Clip gameStartClip = new Sequencer.Clip("GameStart");

            gameStartClip.OnPlay += () =>
            {
                OnSuccessRelay?.Invoke();
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

            gameStartSequence.Append(gameStartClip);
            Sequencer.Append(gameStartSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void UpdateDataRpc(SerializedGameData networkData)
        {
            Sequencer.Sequence updateSequence = new Sequencer.Sequence("Update", Sequencer);
            Sequencer.Clip updateClip = new Sequencer.Clip("UpdateClip");

            updateClip.OnPlay += () =>
            {
                _gameData = networkData.gameData;
                Managers.Logger.Log<GameClient>("Game data updated", colorName: "green");

                OnDataUpdated?.Invoke();
            };

            updateSequence.Append(updateClip);
            Sequencer.Append(updateSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndGameRpc(int clientID)
        {
            Sequencer.Sequence gameEndSequence = new Sequencer.Sequence("GameEnd", Sequencer);
            Sequencer.Clip gameEndClip = new Sequencer.Clip("GameEnd");

            gameEndClip.OnPlay += () =>
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


            gameEndSequence.Append(gameEndClip);
            Sequencer.Append(gameEndSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ResetGameRpc()
        {
            Sequencer.Sequence resetSequence = new Sequencer.Sequence("GameReset", Sequencer);
            Sequencer.Clip resetClipSequence = new Sequencer.Clip("GameReset");

            resetClipSequence.OnPlay += () =>
            {
                Sequencer.Init();
                OnGameReset?.Invoke();
            };

            resetSequence.Append(resetClipSequence);
            Sequencer.Append(resetSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartTurnRpc()
        {
            Sequencer.Sequence turnStartSequence = new Sequencer.Sequence("TurnStart", Sequencer);
            Sequencer.Clip turnStartClip = new Sequencer.Clip("TurnStart");

            turnStartClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>("Turn start", colorName: "green");
                OnTurnStarted?.Invoke();
            };

            turnStartSequence.Append(turnStartClip);
            Sequencer.Append(turnStartSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndTurnRpc()
        {
            Sequencer.Sequence turnEndSequence = new Sequencer.Sequence("TurnEnd", Sequencer);
            Sequencer.Clip turnEndClip = new Sequencer.Clip("TurnEnd");

            turnEndClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>("Turn end", colorName: "green");
                OnTurnEnded?.Invoke();
            };

            turnEndSequence.Append(turnEndClip);
            Sequencer.Append(turnEndSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DrawCardRpc(ulong clientID, string cardUID)
        {
            Sequencer.Sequence cardDrawSequence = new Sequencer.Sequence("CardDraw", Sequencer);
            Sequencer.Clip cardDrawClip = new Sequencer.Clip("CardDraw");

            cardDrawClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>("Card draw", colorName: "green");
                OnCardDrawn?.Invoke(GetGameData().GetPlayer(clientID), GetGameData().GetHandCard(clientID, cardUID));
            };

            cardDrawSequence.Append(cardDrawClip);
            Sequencer.Append(cardDrawSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ChangeManaRpc()
        {
            Sequencer.Sequence changeManaSequence = new Sequencer.Sequence("ChangeMana", Sequencer);
            Sequencer.Clip changeManaClip = new Sequencer.Clip("ChangeMana");

            changeManaClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>("Mana Changed", colorName: "green");
                OnManaChanged?.Invoke();
            };

            changeManaSequence.Append(changeManaClip);
            Sequencer.Append(changeManaSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayCardRpc(bool succeeded, ulong clientID, string cardUID)
        {
            Sequencer.Sequence cardPlaySequence = new Sequencer.Sequence("CardPlay", Sequencer);
            Sequencer.Clip cardPlayClip = new Sequencer.Clip("CardPlay");

            cardPlayClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>($"Card play succeeded? : {succeeded}", colorName: "green");
                OnCardPlayed?.Invoke(succeeded, GetGameData().GetPlayer(clientID), cardUID);
            };

            cardPlaySequence.Append(cardPlayClip);
            Sequencer.Append(cardPlaySequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnCardRpc(bool succeeded, ulong clientID, string cardUID, int index)
        {
            Sequencer.Sequence cardSpawnSequence = new Sequencer.Sequence("CardSpawn", Sequencer);
            Sequencer.Clip cardSpawnClip = new Sequencer.Clip("CardSpawn");

            cardSpawnClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>($"Card spawn succeeded? : {succeeded}", colorName: "green");
                OnCardSpawned?.Invoke(succeeded, GetGameData().GetPlayer(clientID), GetGameData().GetFieldCard(clientID, cardUID), index);
            };

            cardSpawnSequence.Append(cardSpawnClip);
            Sequencer.Append(cardSpawnSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DeadCardRpc()
        {
            Sequencer.Sequence beforeDeadSequence = new Sequencer.Sequence("BeforeDead", Sequencer);
            Sequencer.Clip deadLog = new Sequencer.Clip("DeadLog");
            Sequencer.Clip deadClip = new Sequencer.Clip("Dead");

            deadLog.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>("Card dead");
            };

            deadClip.OnPlay += () =>
            {
                foreach (ICreatureView creature in playerField.Creatures)
                {
                    if (creature.Card.IsDead)
                    {
                        creature.FSM.DeadState.OnComplete += () =>
                        {
                            playerField.Creatures.Remove(creature);
                            Managers.Resource.Destroy(creature.MonoBehaviour.gameObject);
                        };

                        creature.FSM.PushState<CreatureViewDead>();
                    }
                }

                foreach (ICreatureView creature in enemyField.Creatures)
                {
                    if (creature.Card.IsDead)
                    {
                        creature.FSM.DeadState.OnComplete += () =>
                        {
                            enemyField.Creatures.Remove(creature);
                            Managers.Resource.Destroy(creature.MonoBehaviour.gameObject);
                        };

                        creature.FSM.PushState<CreatureViewDead>();
                    }
                }
            };

            beforeDeadSequence.Append(deadLog);
            beforeDeadSequence.Append(deadClip);

            Sequencer.Append(beforeDeadSequence);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartAttackRpc(ulong attackClientID, string attackerUID, ulong defendClientID, string defenderUID)
        {
            Player attackPlayer = GetGameData().GetPlayer(attackClientID);
            Player defendPlayer = GetGameData().GetPlayer(defendClientID);
            Card attacker = attackPlayer.GetCard(attackerUID);
            Card defender = defendPlayer.GetCard(defenderUID);

            Sequencer.Sequence attackSequence = new Sequencer.Sequence("StartAttack", Sequencer);
            Sequencer.Clip attackLogClip = new Sequencer.Clip("AttackLog");
            Sequencer.Clip invokeEventClip = new Sequencer.Clip("StartAttack");

            attackLogClip.OnPlay += () =>
            {
                Managers.Logger.Log<GameClient>("Start Attack", colorName: "green");
            };

            invokeEventClip.OnPlay += () =>
            {
                OnStartAttack?.Invoke(attackSequence, attackPlayer, attacker, defendPlayer, defender);
            };

            attackSequence.Append(attackLogClip);
            attackSequence.Append(invokeEventClip);

            Sequencer.Append(attackSequence);
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

        public ICharacterView GetCharacter(ulong clientID, string cardUID)
        {
            if (IsMine(clientID))
            {
                if (cardUID == PlayerHero.Card.UID)
                {
                    return PlayerHero;
                }
                else
                {
                    return PlayerField.GetCreature(GetGameData().GetFieldCard(clientID, cardUID));
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
                    return EnemyField.GetCreature(GetGameData().GetFieldCard(clientID, cardUID));
                }
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