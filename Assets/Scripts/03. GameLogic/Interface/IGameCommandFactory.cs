namespace Marsion
{
    public interface IGameCommandFactory
    {
        ICommand CreatePlayCardCommand(ulong playerID, string cardUID, int index);
        ICommand CreateAttackCommand(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);
    }

    public class DefaultLogicCommandFactory : IGameCommandFactory
    {
        private IGameDataHandler _dataHandler;
        public DefaultLogicCommandFactory(IGameDataHandler dataHandler)
        {
            _dataHandler = dataHandler;
        }

        public ICommand CreatePlayCardCommand(ulong playerID, string cardUID, int index)
        {
            var _data = new GameCommandData();
            _data.PlayerID = playerID;
            _data.CardUID = cardUID;
            _data.Index = index;

            return new PlayCardCommand(_dataHandler, _data);
        }

        public ICommand CreateAttackCommand(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID)
        {
            var _data = new GameCommandData();
            _data.PlayerID = attackerID;
            _data.CardUID = attackerUID;
            _data.TargetPlayerID = defenderID;
            _data.TargetCardUID = defenderUID;

            return new AttackCommand(_dataHandler, _data);
        }
    }
}