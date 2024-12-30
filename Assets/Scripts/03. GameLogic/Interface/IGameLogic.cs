using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface IGameLogic
    {
        IDataManager Data { get; }
        IGameData GameData { get; }
        event Action<IGameData> OnDataUpdated;
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
        void Clear();
        void StartGame();
        void EndGame();
        void StartTurn();

        // Player Interaction
        void EndTurn();
        void TrySpawnCard(ulong playerID, string cardUID, int index);
        void TryAttack(ulong attackPlayerID, string attackCardUID, ulong defendPlayerID, string defendCardUID);

        // Card Effect
        void DrawCard(ulong playerID, int count = 1);
    }
}