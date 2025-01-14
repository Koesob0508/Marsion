using System;
using System.Collections.Generic;

namespace Marsion
{
    public class RGameLogic : IGameLogic
    {
        public IResourceManager Resource => throw new NotImplementedException();

        public IGameDataHandler DataHandler => throw new NotImplementedException();

        public ITriggerHandler Trigger => throw new NotImplementedException();

        public IGameData GameData => throw new NotImplementedException();

        public ICommandHandler CommandHandler => throw new NotImplementedException();

        public ICommandFactory CommandFactory => throw new NotImplementedException();


        public event Action<IGameData> SendDataUpdated;
        public event Action SendGameStarted;
        public event Action<ulong> SendGameEnded;
        public event Action SendTurnStarted;
        public event Action SendTurnEnded;
        public event Action<ulong, string> SendCardDrawn;
        public event Action<bool, ulong, string, int> SendCardSpawned;
        public event Action<bool, ulong, string, ulong, string> SendCardAttacked;
        public event Action<List<string>> SendCardDied;

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public void DrawCard(ulong playerID, int count = 1)
        {
            throw new NotImplementedException();
        }

        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public void EndTurn()
        {
            throw new NotImplementedException();
        }

        public void Init(IGameLogicFactory gameLogicFactory)
        {
            throw new NotImplementedException();
        }

        public void StartGame()
        {
            throw new NotImplementedException();
        }

        public void StartTurn()
        {
            throw new NotImplementedException();
        }

        public void SubscribeEvent(Action<string, CommandData> observerAlert)
        {
            throw new NotImplementedException();
        }

        public void TryAttack(ulong attackPlayerID, string attackCardUID, ulong defendPlayerID, string defendCardUID)
        {
            throw new NotImplementedException();
        }

        public void TrySpawnCard(ulong playerID, string cardUID, int index)
        {
            throw new NotImplementedException();
        }

        public void UnsubscribeEvent(Action<string, CommandData> observerAlert)
        {
            throw new NotImplementedException();
        }
    }
}