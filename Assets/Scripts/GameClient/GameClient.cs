using Marsion.CardView;
using Marsion.Logic;
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
        public event UnityAction<Player, Card, Player, Card> OnStartAttack;
        public event UnityAction OnCreatureDead;

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
        public void UpdateDataRpc(NetworkGameData networkData)
        {
            Managers.Logger.Log<GameClient>("Game data updated");
            _gameData = networkData.gameData;

            OnDataUpdated?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartGameRpc()
        {
            Managers.Logger.Log<GameClient>("Game Start");

            OnGameStarted?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndGameRpc()
        {
            OnGameEnded?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartTurnRpc()
        {
            OnTurnStarted?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void EndTurnRpc()
        {
            OnTurnEnded?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DrawCardRpc(ulong clientID, string cardUID)
        {
            OnCardDrawn?.Invoke(GetGameData().GetPlayer(clientID), GetGameData().GetHandCard(clientID, cardUID));
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void PlayCardRpc(ulong clientID, string cardUID)
        {
            OnCardPlayed?.Invoke(GetGameData().GetPlayer(clientID), cardUID);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void SpawnCardRpc(ulong clientID, string cardUID, int index)
        {
            OnCardSpawned?.Invoke(GetGameData().GetPlayer(clientID), GetGameData().GetFieldCard(clientID, cardUID), index);
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void DeadCardRpc()
        {
            OnCreatureDead?.Invoke();
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void StartAttackRpc(ulong attackClientID, string attackerUID, ulong defendClientID, string defenderUID)
        {
            Managers.Logger.Log<GameClient>("Start Attack");

            Player attackPlayer = GetGameData().GetPlayer(attackClientID);
            Player defendPlayer = GetGameData().GetPlayer(defendClientID);
            Card attacker = GetGameData().GetFieldCard(attackClientID, attackerUID);
            Card defender = GetGameData().GetFieldCard(defendClientID, defenderUID);

            OnStartAttack?.Invoke(attackPlayer, attacker, defendPlayer, defender);
        }

        #endregion

        public bool TryPlayCard(Card card)
        {
            return true;
        }

        
    }
}