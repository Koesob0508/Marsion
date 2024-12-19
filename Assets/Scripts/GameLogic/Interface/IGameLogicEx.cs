using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface IGameLogicEx
    {
        event Action OnDataUpdated;
        event Action OnGameStarted;
        event Action<ulong> OnGameEnded;
        event Action OnTurnStarted;
        event Action OnTurnEnded;
        event Action OnManaChanged;
        event Action<ulong, string> OnCardDrawn;
        event Action<bool, ulong, string> OnCardPlayed;
        event Action<bool, ulong, string, int> OnCardSpawned;
        event Action<bool, ulong, string, ulong, string> OnCardAttacked;
        event Action<List<string>> OnCardDied;

        void StartGame();
        void EndGame();
        void StartTurn();
        void EndTurn();
        void TrySpawnCard(Player player, Card card, int index);
        void TrySpawnCard(ulong playerID, string cardUID, int index);
        void TryAttack(Player attacker, Card attackerCard, Player defender, Card defenderCard);
        void TryAttack(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);
        void Clear();
    }
}