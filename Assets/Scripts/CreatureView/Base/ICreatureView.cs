using UnityEngine;

namespace Marsion.CardView
{
    public interface ICreatureView : IFSMHandler
    {
        Vector3 OriginPosition { get; set; }
        Card Card { get; }
        MonoBehaviour MonoBehaviour { get; }
        CreatureViewFSM FSM { get; }
        Transform Transform { get; }
        Collider2D Collider { get; }
        IMouseInput Input { get; }
        Order Order { get; }
        Pointer Pointer { get; }

        void Setup(Card card);

        #region State Operations

        void Spawn();

        #endregion

        void UpdateStatus();

        void Attack(Tool.MyTween.Sequence sequence, Player attackPlayer, Card attacker, Player defendPlayer, Card defender);

        void MoveTransform(Vector3 position, bool useDOTween, float dotweenTime = 0);
    }
}