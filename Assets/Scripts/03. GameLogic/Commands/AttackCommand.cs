using System;

namespace Marsion
{
    public class AttackCommand : BaseCommand
    {
        #region Command Data

        private readonly ulong _commanderID;
        private readonly string _commanderCardUID;
        private readonly ulong _targetPlayerID;
        private readonly string _targetCardUID;

        #endregion

        public AttackCommand(IGameLogic logic, ulong commanderID, string commanderCardUID, ulong targetPlayerID, string targetCardUID) : base(logic)
        {
            _commanderID = commanderID;
            _commanderCardUID= commanderCardUID;
            _targetPlayerID = targetPlayerID;
            _targetCardUID = targetCardUID;
        }

        protected override EventData Implement()
        {
            //var attacker = GameLogic.DataHandler.GetCardFromField(attackerID, attackerUID);
            //var defender = GameLogic.DataHandler.GetCardFromField(defenderID, defenderUID);

            //if (attackerUID == null) Logger.LogWarning<AttackCommand>("attack null.");
            //if (defenderUID == null) Logger.LogWarning<AttackCommand>("defend null.");
            //attacker.TakeDamage(defender.Power);
            //defender.TakeDamage(attacker.Power);

            //Logger.Log<DefaultGameLogic>($"{attacker.Name} attacked {defender.Name}", colorName: ColorCodes.Logic);

            return new EventData
            {
                Type = EventType.Attack,
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID,
                    CardUID = _commanderCardUID
                },
                Target = new PlayerAndCard
                {
                    PlayerID = _targetPlayerID,
                    CardUID = _targetCardUID
                }
            };
        }
    }
}