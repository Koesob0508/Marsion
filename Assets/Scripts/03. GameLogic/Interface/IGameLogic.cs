using System;
using System.Collections.Generic;

namespace Marsion
{
    public class CommandData
    {
        public bool Succeeded;
        public ulong PlayerID;
        public string CardUID;
        public ulong TargetPlayerID;
        public string TargetCardUID;
        public int IntValue;

        public List<string> CardUIDs;
    }

    public interface IGameLogic
    {
        IDataManager Data { get; }
        IGameDataHandler DataHandler { get; }
        ITriggerHandler Trigger { get; }
        IGameData GameData { get; }
        ICommandHandler CommandHandler { get; }
        ICommandFactory CommandFactory { get; }

        event Action<IGameData> SendDataUpdated;

        event Action SendGameStarted;
        event Action<ulong> SendGameEnded;
        event Action SendTurnStarted;
        event Action SendTurnEnded;
        event Action<ulong, string> SendCardDrawn;
        event Action<bool, ulong, string, int> SendCardSpawned;
        event Action<bool, ulong, string, ulong, string> SendCardAttacked;
        event Action<List<string>> SendCardDied;

        void Init(IGameLogicFactory gameLogicFactory);
        void SubscribeEvent(Action<string, CommandData> observerAlert);
        void UnsubscribeEvent(Action<string, CommandData> observerAlert);
        void Clear();
        void StartGame();
        void EndGame();
        void StartTurn();

        // Player Interaction
        void EndTurn();

        // TODO : 이후 마법 카드가 나올 수도 있음
        void TrySpawnCard(ulong playerID, string cardUID, int index);
        void TryAttack(ulong attackPlayerID, string attackCardUID, ulong defendPlayerID, string defendCardUID);

        // Card Effect
        void DrawCard(ulong playerID, int count = 1);
    }
}