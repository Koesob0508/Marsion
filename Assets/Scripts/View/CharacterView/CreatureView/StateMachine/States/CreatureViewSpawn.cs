using DG.Tweening;
using UnityEngine;

namespace Marsion.CardView
{
    public class CreatureViewSpawn : BaseCreatureViewState
    {
        Sequence sequence;
        Vector3 DefaultSize = Vector3.one;
        Vector3 HoverSize = new Vector3(2, 2, 1);

        public CreatureViewSpawn(ICharacterView handler, CharacterViewFSM fsm) : base(handler, fsm) { }

        public override void OnInitialize()
        {
            sequence = DOTween.Sequence().Pause()
                .Append(Handler.Transform.DOScale(HoverSize, 0.2f))
                .AppendInterval(0.4f)
                .Append(Handler.Transform.DOScale(DefaultSize, 0.1f))
                .OnComplete(GoToIdle);
        }

        public override void OnEnterState()
        {
            Handler.Order.SetMostFrontOrder(true);
            sequence.Play();
        }

        public override void OnExitState()
        {
        }

        private void GoToIdle()
        {
            FSM.PopState(true);
            Handler.Order.SetMostFrontOrder(false);
            FSM.PushState<CreatureViewIdle>();
        }
    }
}