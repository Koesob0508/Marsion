using System;

namespace Marsion
{
    public class AttackCommand : BaseCommand
    {
        private readonly IGameLogic _gameLogic;
        private readonly CommandData _data;
        private readonly ulong attackerID;
        private readonly string attackerUID;
        private readonly ulong defenderID;
        private readonly string defenderUID;

        public AttackCommand(IGameLogic gameLogic, CommandData data)
        {
            _gameLogic = gameLogic;
            _data = data;

            attackerID = data.PlayerID;
            attackerUID = data.CardUID;
            defenderID = data.TargetPlayerID_2;
            defenderUID = data.TargetCardUID_2;
        }

        public void Execute()
        {
            var attackPlayer = _gameLogic.DataHandler.GetPlayer(attackerID);
            var attacker = _gameLogic.DataHandler.GetCardFromField(attackerID, attackerUID);
            var defenderPlayer = _gameLogic.DataHandler.GetPlayer(defenderID);
            var defender = _gameLogic.DataHandler.GetCardFromField(defenderID, defenderUID);

            if (attackerUID == null) Logger.LogWarning<AttackCommand>("attack null.");
            if (defenderUID == null) Logger.LogWarning<AttackCommand>("defend null.");
            attacker.TakeDamage(defender.Attack);
            defender.TakeDamage(attacker.Attack);

            Logger.Log<DefaultGameLogic>($"{attacker.Name} attacked {defender.Name}", colorName: ColorCodes.Logic);
        }
    }
}