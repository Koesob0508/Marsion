using DG.Tweening;
using UnityEngine;

namespace Marsion.CardView
{
    public class CreatureViewSpawn : BaseCreatureViewState
    {
        Sequence sequence;
        Vector3 DefaultSize = Vector3.one;
        Vector3 HoverSize = new Vector3(2, 2, 1);

        public CreatureViewSpawn(ICreatureView handler, BaseStateMachine fsm) : base(handler, fsm) { }

        public override void OnInitialize()
        {
            sequence = DOTween.Sequence().Pause()
                .Append(Handler.Transform.DOScale(HoverSize, 2))
                .AppendInterval(1f)
                .Append(Handler.Transform.DOScale(DefaultSize, 1f))
                .OnComplete(GoToIdle);
        }

        public override void OnEnterState()
        {
            sequence.Play();
            Managers.Logger.Log<CreatureViewSpawn>("Play");
        }

        public override void OnExitState()
        {
            
        }

        private void GoToIdle()
        {
            FSM.PopState(true);
            FSM.PushState<CreatureViewIdle>();
        }
    }
}