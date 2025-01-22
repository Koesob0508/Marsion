using System;

namespace Marsion
{
    public enum EventType
    {
        None,
        StartGame,
        StartTurn,
        EndTurn,
        PayMana,
        PlayCard,
        CompositionSpell,
        DecompositionSpell,
        SpawnCreatureFromHand,
        AfterSpawn,
        Attack,
        DrawCard,
        BuffAttack,
        Kill,
    }

    public interface IEventHandler
    {
        void Register(EventType type, Action<EventData> listener);
        void Unregister(EventType type, Action<EventData> listener);
        void Trigger(EventData eventData);
    }
}