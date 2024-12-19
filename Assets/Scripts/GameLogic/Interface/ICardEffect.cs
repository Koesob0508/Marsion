using Marsion.Logic;

namespace Marsion
{
    public interface ICardEffect
    {
        /// <summary>
        ///     
        /// </summary>
        /// <param name="source"></param>
        /// <param name="gameData">GameLogicEx를 LogicManager와 DataHandler로 분리시, DataHandler를 넣어줘야함. GameData는 임시방편</param>
        /// <param name="gameLogic"></param>
        void Apply(Card source, GameData gameData, DefaultGameLogic gameLogic);
    }
}