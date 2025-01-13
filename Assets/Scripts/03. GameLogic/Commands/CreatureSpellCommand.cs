using System;
using System.Collections.Generic;

namespace Marsion
{
    public class CreatureSpellCommand : BaseCommand
    {
        private List<AbilitySO> _abilities;

        public CreatureSpellCommand(IGameLogic logic, CommandData data, TriggerType trigger) : base(logic, data, trigger) { }

        public void Register(List<AbilitySO> abilities)
        {
            _abilities = abilities;
        }
        protected override void Implement()
        {
            for (int index = _abilities.Count - 1; index >= 0; index--)
            {
                var effectCommand = GameLogic.CommandFactory.CreateCreatureEffect(CommandData.PlayerID, CommandData.CardUID, _abilities[index].Effect);
                GameLogic.CommandHandler.Add(effectCommand);
            }
        }
    }
}