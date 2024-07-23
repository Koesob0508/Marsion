using UnityEngine;

namespace Marsion
{
    [RequireComponent(typeof(IMouseInput))]
    public class CardView : MonoBehaviour, ICardView
    {
        public string Name => gameObject.name;

        public CardViewFsm FSM { get; private set; }
        public Transform Transform => transform;

        public IMouseInput Input { get; private set; }

        #region Unity Callbacks

        private void Awake()
        {
            // Handler�� IFsmHandler - ICardView �� ���� CardView�� ���޵ȴ�.
            FSM = new CardViewFsm(this);
            Input = GetComponent<IMouseInput>();
        }

        private void Update()
        {
            FSM?.Update();
        }

        #endregion

        #region Operations

        public void Enable() => FSM.PushState<CardViewIdle>();

        #endregion
    }
}