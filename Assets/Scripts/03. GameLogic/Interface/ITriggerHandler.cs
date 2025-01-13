using System;

namespace Marsion
{
    public enum TriggerType
    {
        None,
        Spawn,
        PayMana,
        PlayCreature,
        CreatureSpell,
        Attack,
        DrawCard,
        BuffAttack,
    }

    public interface ITriggerHandler
    {
        void Register(TriggerType type, Action<CommandData> listener);
        void Unregister(TriggerType type, Action<CommandData> listener);
        void Trigger(TriggerType type, CommandData triggerData = null);
    }
}