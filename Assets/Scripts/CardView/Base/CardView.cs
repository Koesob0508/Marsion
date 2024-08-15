using Card = Marsion.Logic.Card;
using UnityEngine;
using TMPro;

namespace Marsion.CardView
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(IMouseInput))]
    public class CardView : MonoBehaviour, ICardView
    {
        #region UI Properties

        [SerializeField] TMP_Text Text_Name;
        [SerializeField] TMP_Text Text_Mana;
        [SerializeField] TMP_Text Text_AbilityExplain;
        [SerializeField] TMP_Text Text_Attack;
        [SerializeField] TMP_Text Text_Health;
        [SerializeField] SpriteRenderer CardSprite;

        #endregion

        #region Properties

        [SerializeField] public CardViewParameters Parameters;

        public Card Card { get; set; }
        public string Name => gameObject.name;
        public MonoBehaviour MonoBehaviour => this;
        public CardViewFsm FSM { get; private set; }
        public Transform Transform { get; set; }
        public Collider Collider { get; set; }
        public IMouseInput Input { get; private set; }
        public Order Order { get; private set; }
        [SerializeField] private GameObject hoverImage;
        [SerializeField] private GameObject backImage;
        public GameObject FrontImage { get => hoverImage; }
        public GameObject BackImage { get => backImage; }

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

            // Handler�� IFsmHandler - ICardView �� ���� CardView�� ���޵ȴ�.
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

        public void Setup()
        {
            Text_Name.text = Card.Name;
            Text_Mana.text = Card.Mana.ToString();
            Text_AbilityExplain.text = Card.AbilityExplain;
            Text_Attack.text = Card.Attack.ToString();
            Text_Health.text = Card.Health.ToString();
            CardSprite.sprite = Managers.Resource.Load<Sprite>(Card.FullArtPath);
        }

        public void Enable() => FSM.PushState<CardViewIdle>();

        public void Draw() => FSM.PushState<CardViewDraw>();

        public void ScaleTo(Vector3 scale, float speed, float delay = 0) => Scale.Execute(scale, speed, delay);

        public void MoveTo(Vector3 position, float speed, float delay = 0) => Position.Execute(position, speed, delay);

        public void MoveToWithZ(Vector3 position, float speed, float delay = 0) => Position.Execute(position, speed, delay, true);

        public void RotateTo(Vector3 euler, float speed, float delay = 0) => Rotation.Execute(euler, speed, delay);

        #endregion
    }
}