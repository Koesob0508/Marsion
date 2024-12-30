using Marsion.Logic;
using System;

namespace Marsion
{
    public interface ICardEffect
    {
        void Execute(IGameLogic gameLogic, IGameDataHandler gameData, Card source);
    }
}