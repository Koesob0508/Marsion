namespace Marsion
{
    public class DamageEffect : ICardEffect
    {
        private int damage;

        public DamageEffect(int damage)
        {
            this.damage = damage;
        }

        public void Execute(IGameLogicEx gameLogic, IGameDataHandler dataHandler, Card source)
        {
            var opponent = dataHandler.GetOpponentPlayer(source.PlayerID);
            //var target = opponent.Field[0];
            //target.TakeDamage(damage);
        }
    }
}