using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface IGameLogicEx
    {
        IGameData GameData { get; }
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

        void Init(IGameLogicFactory gameLogicFactory);
        void StartGame();
        void EndGame();
        void StartTurn();
        void EndTurn();

        void TrySpawnCard(ulong playerID, string cardUID, int index);
        void TryAttack(ulong attackPlayerID, string attackCardUID, ulong defendPlayerID, string defendCardUID);
        void Clear();
    }
}