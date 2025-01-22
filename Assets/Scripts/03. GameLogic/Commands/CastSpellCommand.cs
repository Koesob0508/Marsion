namespace Marsion
{
    public class CastSpellCommand : BaseCommand
    {
        #region

        private readonly ulong _commanderID;
        private readonly string _commanderCardUID;
        private readonly BaseAbility _ability;
        private readonly AbilityType _type;

        #endregion

        public CastSpellCommand(IGameLogic logic, ulong commanderID, string commanderCardUID, BaseAbility ability, AbilityType type) : base(logic)
        {
            _commanderID = commanderID;
            _commanderCardUID = commanderCardUID;
            _ability = ability;
            _type = type;
        }

        protected override EventData Implement()
        {
            Logger.Log<CastSpellCommand>($"Ability {_ability.ID} casted.");

            //_ability.Execute(Logic, abilityData);

            var eventData = new EventData
            {
                Commander = new PlayerAndCard
                {
                    PlayerID = _commanderID,
                    CardUID = _commanderCardUID,
                }
            };

            switch(_type)
            {
                case AbilityType.Composition:
                    eventData.Type = EventType.CompositionSpell;
                    break;
                case AbilityType.Decomposition:
                    eventData.Type = EventType.DecompositionSpell;
                    break;
                case AbilityType.Trigger:
                    eventData.Type = EventType.None;
                    break;
            }

            return eventData;
        }
    }
}   