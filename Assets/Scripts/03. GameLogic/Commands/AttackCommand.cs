namespace Marsion
{
    public class AttackCommand : ICommand
    {
        private readonly Player attackerPlayer;
        private readonly Card attackerCard;
        private readonly Player defenderPlayer;
        private readonly Card defenderCard;

        public AttackCommand(Player attackerPlayer, Card attackerCard, Player defenderPlayer, Card defenderCard)
        {
            this.attackerPlayer = attackerPlayer;
            this.attackerCard = attackerCard;
            this.defenderPlayer = defenderPlayer;
            this.defenderCard = defenderCard;
        }

        public void Execute(IGameDataHandler datahandler)
        {
            if (attackerCard == null) Logger.LogWarning<AttackCommand>("attack null.");
            if (defenderCard == null) Logger.LogWarning<AttackCommand>("defend null.");
            attackerCard.TakeDamage(defenderCard.Attack);
            defenderCard.TakeDamage(attackerCard.Attack);

            Logger.Log<DefaultGameLogic>($"{attackerCard.Name} attacked {defenderCard.Name}", colorName: ColorCodes.Logic);
        }
    }
}