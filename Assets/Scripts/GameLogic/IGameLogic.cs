using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion.Logic
{
    public interface IGameLogic
    {
        GameData GetGameData();

        #region Unity Actions

        event UnityAction OnDataUpdated;
        event UnityAction OnGameStarted;
        event UnityAction<int> OnGameEnded;
        event UnityAction OnTurnStarted;
        event UnityAction OnTurnEnded;
        event UnityAction<Player, Card> OnCardDrawn;
        event UnityAction OnManaChanged;
        event UnityAction <bool, Player, Card> OnCardPlayed;
        event UnityAction <bool, Player, Card, int> OnCardSpawned;
        event UnityAction<Card, Card> OnStartAttack;
        event UnityAction OnCardBeforeDead;
        event UnityAction OnCardAfterDead;

        #endregion

        #region Game Flow

        void StartGame();
        
        void EndGame();

        void StartTurn();
        
        void EndTurn();

        #endregion

        #region Game Logic Operations

        void ShuffleDeck(List<Card> deck);

        void DrawCard(Player player);

        void PlayCard(Player player, Card card);

        void SpanwCard(Player player, Card card, int index);

        void TryPlayAndSpawnCard(Player player, Card card, int index);

        void TryAttack(Player attackPlayer, Card attacker, Player defenderPlayer, Card defender);

        #endregion
    }
}