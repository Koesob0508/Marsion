namespace Marsion
{
    public class CastSpellCommand : BaseCommand
    {
        AbilitySO Ability;
        EventData EventData;

        public CastSpellCommand(IGameLogic logic, AbilitySO ability, EventData eventData) : base(logic)
        {
            Ability = ability;
            EventData = eventData;
            EventType = EventType.CastSpell;
        }

        protected override void Implement()
        {
            Logger.Log<CastSpellCommand>($"Ability {Ability.ID} casted.");

            Ability.PrepareCast(Logic, EventData);
            Ability.Execute();
        }
    }
}