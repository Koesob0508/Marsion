namespace Marsion
{
    public interface ICommandFactory
    {
        ICommand CreatePayMana(ulong playerID, int amount);
        ICommand CreatePlayCard(ulong playerID, string cardUID, int index);
        ICommand CreateAttack(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);
    }

    public class DefaultCommandFactory : ICommandFactory
    {
        private IGameLogic _gameLogic;
        public DefaultCommandFactory(IGameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public ICommand CreatePayMana(ulong playerID, int amount)
        {
            var data = new CommandData();
            data.PlayerID = playerID;
            data.IntValue = amount;

            return new PayManaCommand(_gameLogic, data);
        }

        public ICommand CreatePlayCard(ulong playerID, string cardUID, int index)
        {
            var data = new CommandData();
            data.PlayerID = playerID;
            data.CardUID = cardUID;
            data.IntValue = index;

            return new PlayCardCommand(_gameLogic, data);
        }

        public ICommand CreateAttack(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID)
        {
            var data = new CommandData();
            data.PlayerID = attackerID;
            data.CardUID = attackerUID;
            data.TargetPlayerID_2 = defenderID;
            data.TargetCardUID_2 = defenderUID;

            return new AttackCommand(_gameLogic, data);
        }
    }
}