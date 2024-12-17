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

        public void Execute()
        {
            attackerCard.Damage(defenderCard.Attack);
            defenderCard.Damage(attackerCard.Attack);

            Logger.Log<GameLogicEx>($"{attackerCard.Name} attacked {defenderCard.Name}", colorName: ColorCodes.Logic);
        }
    }
}