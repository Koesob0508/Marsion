using System;

namespace Marsion
{
    public class AttackCommand : ICommand
    {
        public event Action<GameCommandData> OnCompleted;

        private readonly IGameDataHandler _dataHandler;
        private readonly GameCommandData _data;
        private readonly ulong attackerID;
        private readonly string attackerUID;
        private readonly ulong defenderID;
        private readonly string defenderUID;

        public AttackCommand(IGameDataHandler dataHandler, GameCommandData data)
        {
            _dataHandler = dataHandler;
            _data = data;

            attackerID = data.PlayerID;
            attackerUID = data.CardUID;
            defenderID = data.TargetPlayerID;
            defenderUID = data.TargetCardUID;
        }

        public void Execute()
        {
            var attackPlayer = _dataHandler.GetPlayer(attackerID);
            var attacker = _dataHandler.GetCardFromField(attackerID, attackerUID);
            var defenderPlayer = _dataHandler.GetPlayer(defenderID);
            var defender = _dataHandler.GetCardFromField(defenderID, defenderUID);

            if (attackerUID == null) Logger.LogWarning<AttackCommand>("attack null.");
            if (defenderUID == null) Logger.LogWarning<AttackCommand>("defend null.");
            attacker.TakeDamage(defender.Attack);
            defender.TakeDamage(attacker.Attack);

            Logger.Log<DefaultGameLogic>($"{attacker.Name} attacked {defender.Name}", colorName: ColorCodes.Logic);

            OnCompleted?.Invoke(_data);
        }
        public void Clear()
        {
            OnCompleted = null;
        }
    }
}