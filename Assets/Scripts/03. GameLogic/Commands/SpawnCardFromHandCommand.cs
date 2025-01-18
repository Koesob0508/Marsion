namespace Marsion
{
    public class SpawnCardFromHandCommand : BaseCommand
    {
        private ulong playerID;
        private string cardUID;
        private int index;

        public SpawnCardFromHandCommand(IGameLogic logic, ulong playerID, string cardUID, int index) : base(logic)
        {
            this.playerID = playerID;
            this.cardUID = cardUID;
            this.index = index;

            EventType = EventType.SpawnCardFromHand;
        }

        protected override void Implement()
        {
            Logger.Log<IGameLogic>($"Player {playerID} spawn card {cardUID} from hand.", colorName: ColorCodes.Logic);

            if(Logic.DataHandler.TryGetCardFromHand(playerID, cardUID, out var card))
            {
                Logic.DataHandler.RemoveCardFromHand(playerID, cardUID);
                Logic.DataHandler.AddCardToField(playerID, card, index);
            }
            else
            {
                Logger.Log<IGameLogic>($"Try spawn card from hand. But the player {playerID} did not have card {cardUID} in hand.", colorName: ColorCodes.Logic);
            }
        }
    }
}