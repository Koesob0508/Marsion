namespace Marsion
{
    public class AddSpellCommand : BaseCommand
    {
        ulong playerID;
        string cardUID;

        public AddSpellCommand(IGameLogic logic, ulong playerID, string cardUID) : base(logic)
        {
            this.playerID = playerID;
            this.cardUID = cardUID;
        }

        protected override void Implement()
        {
            Logic.DataHandler.TryGetCardFromHand(playerID, cardUID, out var targetCard);

            Logger.Log<AddSpellCommand>($"Cast creature spell {targetCard.Abilities.Count}");

            foreach (var ability in targetCard.Abilities)
            {
                if (ability.Type == AbilityType.Spell)
                {
                    EventData eventData = new();
                    eventData.PlayerID = playerID;
                    eventData.CardUID = cardUID;

                    Logic.CommandHandler.Add(Logic.CommandFactory.CreateCastSpell(ability, eventData));
                }
            }
        }
    }
}