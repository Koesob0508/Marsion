namespace Marsion
{
    public class PlayCardCommand : BaseCommand
    {
        private ulong playerID;
        private string cardUID;
        private int index;

        public PlayCardCommand(IGameLogic logic, ulong playerID, string cardUID, int index) : base(logic)
        {
            EventType = EventType.PlayCard;

            this.playerID = playerID;
            this.cardUID = cardUID;
            this.index = index;
        }

        protected override void Implement()
        {
            Logger.Log<IGameLogic>("Try spawn card", colorName: ColorCodes.Logic);

            Logic.DataHandler.TryGetCardFromHand(playerID, cardUID, out var card);

            // 순서대로 작성한 후, 역순으로 Add할 것
            // 마나를 소모
            var payMana = Logic.CommandFactory.CreatePayMana(playerID, card.ManaCost);

            // 해당 카드의 CreatureSpell Cast
            var castSpell = Logic.CommandFactory.CreateAddSpell(playerID, cardUID);

            // 해당 카드 소환(손에서 낼때)
            var spawnCard = Logic.CommandFactory.CreateSpawnCardFromHand(playerID, cardUID, index);

            Logic.CommandHandler.Add(spawnCard);
            Logic.CommandHandler.Add(castSpell);
            Logic.CommandHandler.Add(payMana);
        }
    }
}