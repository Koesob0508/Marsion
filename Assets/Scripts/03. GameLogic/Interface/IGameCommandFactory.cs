namespace Marsion
{
    public interface IGameCommandFactory
    {
        ICommand CreatePlayCardCommand(ulong playerID, string cardUID, int index);
        ICommand CreateAttackCommand(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);
    }

    public class DefaultGameCommandFactory : IGameCommandFactory
    {
        private IGameLogic _gameLogic;
        public DefaultGameCommandFactory(IGameLogic gameLogic)
        {
            _gameLogic = gameLogic;
        }

        public ICommand CreatePlayCardCommand(ulong playerID, string cardUID, int index)
        {
            var _data = new GameCommandData();
            _data.PlayerID = playerID;
            _data.CardUID = cardUID;
            _data.Index = index;

            return new PlayCardCommand(_gameLogic, _data);
        }

        public ICommand CreateAttackCommand(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID)
        {
            var _data = new GameCommandData();
            _data.PlayerID = attackerID;
            _data.CardUID = attackerUID;
            _data.TargetPlayerID = defenderID;
            _data.TargetCardUID = defenderUID;

            return new AttackCommand(_gameLogic, _data);
        }
    }
}