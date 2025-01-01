using System;
using System.Collections.Generic;

namespace Marsion
{
    public struct LogicCommandData
    {
        public bool Success;
        public ulong PlayerID;
        public string CardUID;
        public ulong TargetPlayerID;
        public string TargetCardUID;
        public int Index;

        public List<string> CardUIDs;
    }

    public interface IGameLogic
    {
        IDataManager Data { get; }
        IGameDataHandler DataHandler { get; }
        GameEventHandler Event { get; }
        IGameData GameData { get; }

        event Action<IGameData> SendDataUpdated;

        event Action SendGameStarted;
        event Action<ulong> SendGameEnded;
        event Action SendTurnStarted;
        event Action SendTurnEnded;
        event Action SendManaChanged;
        event Action<ulong, string> SendCardDrawn;
        event Action<bool, ulong, string> SendCardPlayed;
        event Action<bool, ulong, string, int> SendCardSpawned;
        event Action<bool, ulong, string, ulong, string> SendCardAttacked;
        event Action<List<string>> SendCardDied;

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