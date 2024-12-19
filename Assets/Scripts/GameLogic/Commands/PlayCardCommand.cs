namespace Marsion
{
    public class PlayCardCommand : ICommand
    {
        private readonly Player player;
        private readonly Card card;
        private readonly int index;

        public PlayCardCommand(Player player, Card card, int index)
        {
            this.player = player;
            this.card = card;
            this.index = index;
        }

        public void Execute()
        {
            if(player.Mana < card.Mana)
            {
                Logger.Log<DefaultGameLogic>($"Not enoufh mana to play {card.Name}", colorName: ColorCodes.Logic);
                return;
            }
            player.PayMana(card.Mana);
            player.Hand.Remove(card);
            player.Field.Insert(index, card);

            Logger.Log<DefaultGameLogic>($"Played card : {card.Name} at position {index}", colorName: ColorCodes.Logic);
        }
    }
}