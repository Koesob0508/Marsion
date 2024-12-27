using Marsion.Logic;

namespace Marsion
{
    public interface ICardEffect
    {
        void Execute(IGameLogic gameLogic, IGameDataHandler gameData, Card source);
    }
}