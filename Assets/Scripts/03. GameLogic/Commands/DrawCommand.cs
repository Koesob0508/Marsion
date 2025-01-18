namespace Marsion
{
    public class DrawCommand : BaseCommand
    {
        private ulong playerID;
        private int count;

        public DrawCommand(IGameLogic logic, ulong targetPlayerID, int count) : base(logic)
        {
            playerID = targetPlayerID;
            this.count = count;
            EventData = new();
            EventType = EventType.DrawCard;
        }
        protected override void Implement()
        {
            Logic.DataHandler.DrawCard(playerID, out var drawnCards, count: count);

            EventData.CardUIDs = new();
            foreach(var card in drawnCards)
            {
                EventData.CardUIDs.Add(card.UID);
            }
        }
    }
}