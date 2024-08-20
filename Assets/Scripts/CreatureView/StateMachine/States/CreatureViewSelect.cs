using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Marsion.CardView
{
    public class CreatureViewSelect : BaseCreatureViewState
    {
        Plane plane;
        Sequence sequence;
        Vector3 DefaultSize = Vector3.one;
        Vector3 HoverSize = new Vector3(1.5f, 1.5f, 1);

        public CreatureViewSelect(ICreatureView handler, CreatureViewFSM fsm) : base(handler, fsm) { }

        #region State Operations

        public override void OnInitialize()
        {
            sequence = DOTween.Sequence().SetAutoKill(false).Pause()
                .Append(Handler.Transform.DOScale(HoverSize, 0.2f));
        }

        public override void OnEnterState()
        {
            FSM.ActivatePointer("CreatureView", OnPointerClick);
            Handler.Order.SetMostFrontOrder(true);
            sequence.Play();
        }

        public override void OnExitState()
        {
            FSM.DeactivatePointer();
        }

        #endregion

        private void OnPointerClick(bool isDetected, GameObject foundObject)
        {
            Managers.Logger.Log<CreatureViewSelect>($"{isDetected} & {foundObject.name}");
        }
    }
}