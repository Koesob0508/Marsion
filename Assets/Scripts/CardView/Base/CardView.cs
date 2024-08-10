using UnityEngine;

namespace Marsion
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(IMouseInput))]
    public class CardView : MonoBehaviour, ICardView
    {
        #region Properties

        [SerializeField] public CardViewParameters Parameters;

        public string Name => gameObject.name;
        public MonoBehaviour MonoBehaviour => this;
        public CardViewFsm FSM { get; private set; }
        public Transform Transform { get; set; }
        public Collider Collider { get; set; }
        public IMouseInput Input { get; private set; }
        public Order Order { get; private set; }
        [SerializeField] private GameObject hoverImage;
        GameObject ICardView.HoverImage { get => hoverImage; }

        public BaseCardViewMotion Scale { get; private set; }
        public BaseCardViewMotion Position { get; private set; }
        public BaseCardViewMotion Rotation { get; private set; }
        

        #endregion

        #region Unity Callbacks

        private void Awake()
        {
            Transform = transform;
            Collider = GetComponent<Collider>();

            Input = GetComponent<IMouseInput>();
            Order = GetComponent<Order>();

            Scale = new ScaleCardViewMotion(this);
            Position = new PositionCardViewMotion(this);
            Rotation = new RotationCardViewMotion(this);

            // Handler로 IFsmHandler - ICardView 를 받은 CardView가 전달된다.
            FSM = new CardViewFsm(Parameters, this);
        }

        private void Update()
        {
            FSM?.Update();
            Scale?.Update();
            Position?.Update();
            Rotation?.Update();
        }

        #endregion

        #region Operations

        public void Enable() => FSM.PushState<CardViewIdle>();

        public void Draw() => FSM.PushState<CardViewDraw>();

        public void ScaleTo(Vector3 scale, float speed, float delay = 0) => Scale.Execute(scale, speed, delay);

        public void MoveTo(Vector3 position, float speed, float delay = 0) => Position.Execute(position, speed, delay);

        public void MoveToWithZ(Vector3 position, float speed, float delay = 0) => Position.Execute(position, speed, delay, true);

        public void RotateTo(Vector3 euler, float speed, float delay = 0) => Rotation.Execute(euler, speed, delay);

        #endregion
    }
}