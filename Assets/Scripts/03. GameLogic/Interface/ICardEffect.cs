using Marsion.Logic;

namespace Marsion
{
    public interface ICardEffect
    {
        void Execute(IGameLogicEx gameLogic, IGameDataHandler gameData, Card source);
    }
}