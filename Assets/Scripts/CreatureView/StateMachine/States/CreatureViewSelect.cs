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
            sequence = DOTween.Sequence();


        }

        public override void OnEnterState()
        {
            FSM.ActivatePointer("CreatureView", OnPointerUp);
            Handler.Order.SetMostFrontOrder(true);
            Handler.Transform.DOScale(HoverSize, 0.2f);
        }

        public override void OnExitState()
        {
            Handler.Transform.DOScale(DefaultSize, 0.1f);
            FSM.DeactivatePointer();
        }

        #endregion

        private void OnPointerUp(bool isDetected, GameObject foundObject)
        {
            if (foundObject == null)
            {
                Managers.Logger.Log<CreatureViewSelect>($"{isDetected} & null");
                FSM.PopState();
            }
            else
            {
                ICreatureView target = foundObject.GetComponent<CreatureView>();

                if (Handler.Card.ClientID != target.Card.ClientID)
                {
                    Managers.Logger.Log<CreatureViewSelect>("Client : 공격");
                    FSM.Target = foundObject;
                    FSM.PopState();
                    //FSM.PushState<CreatureViewAttack>();
                    Managers.Client.TryAttack(Handler.Card, foundObject.GetComponent<ICreatureView>().Card);
                }
                else
                {
                    FSM.PopState();
                }
            }
        }
    }
}