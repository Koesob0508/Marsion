using Marsion.CardView;
using Marsion.Logic;
using UnityEngine;
using UnityEngine.Events;
using Marsion.Tool;
using System;

namespace Marsion.Client
{
    /// <summary>
    ///     Client의 Game Logic
    /// </summary>
    public interface IGameClient
    {
        ulong ID { get; }
        ulong EnemyID { get; }
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
        event UnityAction OnManaChanged;
        event UnityAction<bool, Player, string> OnCardPlayed;
        event UnityAction<bool, Player, Card, int> OnCardSpawned;
        event Action<MyTween.Sequence, Player, Card, Player, Card> OnStartAttack;
        event Action<MyTween.Sequence> OnCharacterBeforeDead;
        event UnityAction OnCharacterAfterDead;

        #endregion

        #region Get Operations

        bool IsMine(Player player);
        bool IsMine(Card card);
        bool IsMine(ulong id);
        bool IsMyTurn();
        GameData GetGameData();
        ICharacterView GetCreature(ulong clientID, string cardUID);
        Sprite GetPortrait(int index);

        Card GetCard(CardType type, ulong clientID, string cardUID);

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
        void TryPlayAndSpawnCard(Card card, int index);
        void TurnEnd();
        void TryAttack(Card attacker, Card defender);

        #endregion

        #region Event Rpcs

        void UpdateDataRpc(NetworkGameData networkData);
        void StartGameRpc();
        void EndGameRpc(int clientID);
        void StartTurnRpc();
        void EndTurnRpc();
        void DrawCardRpc(ulong clientID, string cardUID);
        void ChangeManaRpc();
        void PlayCardRpc(bool succeeded, ulong clientID, string cardUID);
        void SpawnCardRpc(bool succeeded, ulong clientID, string cardUID, int index);

        void BeforeDeadCardRpc();
        void AfterDeadCardRpc();

        void StartAttackRpc(ulong attackClientID, string attackerUID, ulong defendClientID, string defenderUID);

        #endregion
    }
}