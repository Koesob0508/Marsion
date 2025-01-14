using UnityEngine;

namespace Marsion.CardView
{
    public interface ICharacterView : IFSMHandler
    {
        ICard Card { get; }
        MonoBehaviour MonoBehaviour { get; }
        CharacterViewFSM FSM { get; }
        Transform Transform { get; }
        Collider2D Collider { get; }
        IMouseInput Input { get; }
        Order Order { get; }
        Pointer Pointer { get; }

        void Init(ICard card);
        void Clear();
        void Spawn();
        void Die();
        void UpdateStatus();
    }
}