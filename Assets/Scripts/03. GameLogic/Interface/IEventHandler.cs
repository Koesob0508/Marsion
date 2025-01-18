using System;

namespace Marsion
{
    public enum EventType
    {
        None,
        StartGame,
        StartTurn,
        EndTurn,
        Spawn,
        PayMana,
        PlayCard,
        CastSpell,
        SpawnCardFromHand,
        Attack,
        DrawCard,
        BuffAttack,
    }

    public interface IEventHandler
    {
        void Register(EventType type, Action<EventData> listener);
        void Unregister(EventType type, Action<EventData> listener);
        void Trigger(EventType type, EventData eventData = null);
    }
}