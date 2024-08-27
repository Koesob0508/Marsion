using Marsion.CardView;
using Marsion.Logic;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion.Client
{
    /// <summary>
    ///     Client의 Game Logic
    /// </summary>
    public interface IGameClient
    {
        ulong ID { get; }
        IHandView Hand { get; }
        IFieldView PlayerField { get; }
        IFieldView EnemyField { get; }
        InputManager Input { get; }

        

        #region Events

        event UnityAction OnDataUpdated;
        event UnityAction OnGameStarted;
        event UnityAction OnGameEnded;
        event UnityAction OnTurnStarted;
        event UnityAction OnTurnEnded;
        event UnityAction<Player, Card> OnCardDrawn;
        event UnityAction<Player, string> OnCardPlayed;
        event UnityAction<Player, Card, int> OnCardSpawned;
        event UnityAction<Player, Card, Player, Card> OnStartAttack;
        event UnityAction OnCreatureDead;

        #endregion

        #region Get Operations

        bool IsMine(Player player);
        bool IsMine(Card card);
        bool IsMine(ulong id);
        bool IsMyTurn();
        GameData GetGameData();
        ICreatureView GetCreature(ulong clientID, string cardUID);
        Sprite GetPortrait(int index);

        #endregion

        #region Manager Operations

        void Init();
        void Update();
        void Clear();

        #endregion

        #region Client Operations

        void SetClientID(ulong clientID);
        void DrawCard();
        void PlayCard(Card card);
        void PlayAndSpawnCard(Card card, int index);
        bool TryPlayCard(Card card);
        void TurnEnd();
        void TryAttack(Card attacker, Card defender);

        #endregion

        #region Event Rpcs

        void UpdateDataRpc(NetworkGameData networkData);
        void StartGameRpc();
        void EndGameRpc();
        void StartTurnRpc();
        void EndTurnRpc();
        void DrawCardRpc(ulong clientID, string cardUID);
        void PlayCardRpc(ulong clientID, string cardUID);
        void SpawnCardRpc(ulong clientID, string cardUID, int index);

        void DeadCardRpc();
        void StartAttackRpc(ulong attackClientID, string attackerUID, ulong defendClientID, string defenderUID);

        #endregion
    }
}