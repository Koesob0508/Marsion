using Marsion.Logic;

namespace Marsion
{
    public class DrawCardEffect : ICardEffect
    {
        private readonly int _drawCount;

        public DrawCardEffect(int drawCount)
        {
            _drawCount = drawCount;
        }

        public void Apply(Card source, DefaultGameData gameData, DefaultGameLogic gameLogic)
        {
            //var player = gameData.GetPlayer(source.OwnerID);
            //for (int i = 0; i < _drawCount; i++)
            //{
            //    gameLogic.DrawCard(player, out _);
            //}

            //Logger.Log<DrawCardEffect>($"Player {player.PlayerID} drew {_drawCount} cards.");
        }
    }

    //var card = new Card();
    //var drawEffect = new DrawCardEffect(2);
    //var turnStartTrigger = new TurnStartTrigger(card, drawEffect);

    //card.AddTrigger(turnStartTrigger);
}