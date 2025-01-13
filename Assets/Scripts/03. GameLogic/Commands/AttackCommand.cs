using System;

namespace Marsion
{
    public class AttackCommand : BaseCommand
    {
        private readonly ulong attackerID;
        private readonly string attackerUID;
        private readonly ulong defenderID;
        private readonly string defenderUID;

        public AttackCommand(IGameLogic logic, CommandData data, TriggerType trigger) : base(logic, data, trigger)
        {
            attackerID = data.PlayerID;
            attackerUID = data.CardUID;
            defenderID = data.TargetPlayerID;
            defenderUID = data.TargetCardUID;
        }

        protected override void Implement()
        {
            var attacker = GameLogic.DataHandler.GetCardFromField(attackerID, attackerUID);
            var defender = GameLogic.DataHandler.GetCardFromField(defenderID, defenderUID);

            if (attackerUID == null) Logger.LogWarning<AttackCommand>("attack null.");
            if (defenderUID == null) Logger.LogWarning<AttackCommand>("defend null.");
            attacker.TakeDamage(defender.Attack);
            defender.TakeDamage(attacker.Attack);

            Logger.Log<DefaultGameLogic>($"{attacker.Name} attacked {defender.Name}", colorName: ColorCodes.Logic);
        }
    }
}