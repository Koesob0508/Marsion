using UnityEngine.EventSystems;

namespace Marsion.CardView
{
    public class CreatureViewIdle : BaseCreatureViewState
    {
        public CreatureViewIdle(ICreatureView handler, CreatureViewFSM fsm) : base(handler, fsm) { }

        #region State Operations

        public override void OnEnterState()
        {
            Handler.Input.OnPointerClick -= OnPointerClick;
            Handler.Input.OnPointerClick += OnPointerClick;
        }

        #endregion

        #region Pointer Operations

        private void OnPointerClick(PointerEventData eventData)
        {
            if(FSM.IsCurrent(this) && Managers.Client.IsMyTurn() && eventData.button == PointerEventData.InputButton.Left)
            {
                FSM.PushState<CreatureViewSelect>();
            }
        }

        #endregion 
    }
}