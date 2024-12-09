using Marsion.CardView;
using Marsion.Logic;
using Marsion.Tool;
using Marsion.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


namespace Marsion.Client
{
    public class GameClient : NetworkBehaviour
    {
        public List<Sprite> PortraitSprites;

        public GameData Data;
        
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

        public event Action OnSuccessRelay;
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
            Logger.Log<GameClient>($"Game Client initialized", colorName: ColorCodes.Client);

            Sequencer.Init();

            Managers.Server.Game.OnDataUpdated += UpdateDataRpc;
            Managers.Server.Game.OnGameStarted += StartGameRpc;
            Managers.Server.Game.OnGameEnded += EndGameRpc;
            Managers.Server.Game.OnResetGame += ResetGameRpc;
            Managers.Server.Game.OnTurnStarted += StartTurnRpc;
            Managers.Server.Game.OnTurnEnded += EndTurnRpc;
            Managers.Server.Game.OnCardDrawn += DrawCardRpc;
            Managers.Server.Game.OnManaChanged += ChangeManaRpc;
            Managers.Server.Game.OnCardPlayed += PlayCardRpc;
            Managers.Server.Game.OnCardSpawned += SpawnCardRpc;
            Managers.Server.Game.OnStartAttack += StartAttackRpc;
            Managers.Server.Game.OnDeadCard += DeadCardRpc;
        }

        public void Clear()
        {

        }

        public void Ready(List<string> deck)
        {
            List<StringContainer> sdata = new();
            foreach (var id in deck)
            {
                StringContainer container = new();
                container.SomeText = id;
                sdata.Add(container);
            }

            Managers.Server.Game.ReadyRpc(ID, sdata.ToArray());
        }

        public void TryPlayAndSpawnCard(Card card, int index)
        {
            Managers.Server.Game.TryPlayAndSpawnCardRpc(ID, card.UID, index);
        }

        public void TurnEnd()
        {
            Managers.Server.Game.TurnEndRpc();
        }

        public void TryAttack(Card attacker, Card defender)
        {
            Managers.Server.Game.TryAttackRpc(attacker.PlayerID, attacker.UID, defender.PlayerID, defender.UID);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartGameRpc()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("GameStart", Sequencer);
            Sequencer.Clip gameStartClip = new Sequencer.Clip("GameStart", sequence);

            gameStartClip.OnPlay += () =>
            {
                OnSuccessRelay?.Invoke();
                Logger.Log<GameClient>("Start game", colorName: "green");

                foreach (var player in GetGameData().Players)
                {
                    if (ID == player.PlayerID)
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

                OnGameStarted?.Invoke();
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void UpdateDataRpc(SerializedGameData networkData)
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("Update", Sequencer);
            Sequencer.Clip updateClip = new Sequencer.Clip("UpdateClip", sequence);

            updateClip.OnPlay += () =>
            {
                Data = networkData.GameData;
                Logger.Log<GameClient>("Game data updated", colorName: "green");

                OnDataUpdated?.Invoke();
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndGameRpc(int clientID)
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("GameEnd", Sequencer);
            Sequencer.Clip gameEndClip = new Sequencer.Clip("GameEnd", sequence);

            gameEndClip.OnPlay += () =>
            {
                Logger.Log<GameClient>("Game end.");
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
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ResetGameRpc()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("GameReset", Sequencer);
            Sequencer.Clip resetClipSequence = new Sequencer.Clip("GameReset",sequence);

            resetClipSequence.OnPlay += () =>
            {
                Sequencer.Init();
                OnGameReset?.Invoke();
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartTurnRpc()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("TurnStart", Sequencer);
            Sequencer.Clip turnStartClip = new Sequencer.Clip("TurnStart", sequence);

            turnStartClip.OnPlay += () =>
            {
                Logger.Log<GameClient>("Turn start", colorName: "green");
                OnTurnStarted?.Invoke();
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndTurnRpc()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("TurnEnd", Sequencer);
            Sequencer.Clip turnEndClip = new Sequencer.Clip("TurnEnd", sequence);

            turnEndClip.OnPlay += () =>
            {
                Logger.Log<GameClient>("Turn end", colorName: "green");
                OnTurnEnded?.Invoke();
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DrawCardRpc(ulong clientID, string cardUID)
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("CardDraw", Sequencer);
            Sequencer.Clip cardDrawClip = new Sequencer.Clip("CardDraw", sequence);

            cardDrawClip.OnPlay += () =>
            {
                Logger.Log<GameClient>("Card draw", colorName: "green");
                OnCardDrawn?.Invoke(GetGameData().GetPlayer(clientID), GetGameData().GetHandCard(clientID, cardUID));
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void ChangeManaRpc()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("ChangeMana", Sequencer);
            Sequencer.Clip changeManaClip = new Sequencer.Clip("ChangeMana", sequence);

            changeManaClip.OnPlay += () =>
            {
                Logger.Log<GameClient>("Mana Changed", colorName: "green");
                OnManaChanged?.Invoke();
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayCardRpc(bool succeeded, ulong clientID, string cardUID)
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("CardPlay", Sequencer);
            Sequencer.Clip cardPlayClip = new Sequencer.Clip("CardPlay", sequence);

            cardPlayClip.OnPlay += () =>
            {
                Logger.Log<GameClient>($"Card play succeeded? : {succeeded}", colorName: "green");
                OnCardPlayed?.Invoke(succeeded, GetGameData().GetPlayer(clientID), cardUID);
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnCardRpc(bool succeeded, ulong clientID, string cardUID, int index)
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("CardSpawn", Sequencer);
            Sequencer.Clip cardSpawnClip = new Sequencer.Clip("CardSpawn", sequence);

            cardSpawnClip.OnPlay += () =>
            {
                Logger.Log<GameClient>($"Card spawn succeeded? : {succeeded}", colorName: "green");
                OnCardSpawned?.Invoke(succeeded, GetGameData().GetPlayer(clientID), GetGameData().GetFieldCard(clientID, cardUID), index);
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DeadCardRpc()
        {
            Sequencer.Sequence sequence = new Sequencer.Sequence("BeforeDead", Sequencer);
            Sequencer.Clip deadLog = new Sequencer.Clip("DeadLog", sequence);
            Sequencer.Clip deadClip = new Sequencer.Clip("Dead", sequence);

            deadLog.OnPlay += () =>
            {
                Logger.Log<GameClient>("Card dead");
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
                            Managers.Instance.Resource.Destroy(creature.MonoBehaviour.gameObject);
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
                            Managers.Instance.Resource.Destroy(creature.MonoBehaviour.gameObject);
                        };

                        creature.FSM.PushState<CreatureViewDead>();
                    }
                }
            };
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartAttackRpc(ulong attackClientID, string attackerUID, ulong defendClientID, string defenderUID)
        {
            Player attackPlayer = GetGameData().GetPlayer(attackClientID);
            Player defendPlayer = GetGameData().GetPlayer(defendClientID);
            Card attacker = attackPlayer.GetCard(attackerUID);
            Card defender = defendPlayer.GetCard(defenderUID);

            Sequencer.Sequence sequence = new Sequencer.Sequence("StartAttack", Sequencer);
            Sequencer.Clip attackLogClip = new Sequencer.Clip("AttackLog", sequence);
            Sequencer.Clip invokeEventClip = new Sequencer.Clip("StartAttack", sequence);

            attackLogClip.OnPlay += () =>
            {
                Logger.Log<GameClient>("Start Attack", colorName: "green");
            };

            invokeEventClip.OnPlay += () =>
            {
                OnStartAttack?.Invoke(sequence, attackPlayer, attacker, defendPlayer, defender);
            };
        }

        #region Utils

        public bool IsMine(Player player)
        {
            return Data.GetPlayer(ID) == player;
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
            return Data;
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