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
        event UnityAction OnGameEnded;
        event UnityAction OnTurnStarted;
        event UnityAction OnTurnEnded;
        event UnityAction<Player, Card> OnCardDrawn;
        event UnityAction <Player, Card> OnCardPlayed;
        event UnityAction <Player, Card, int> OnCardSpawned;

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

        void PlayAndSpawnCard(Player player, Card card, int index);

        #endregion
    }
}