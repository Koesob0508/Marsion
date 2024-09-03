using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(menuName = "Card View Configuration")]
    public class CardViewParameters : ScriptableObject
    {
        #region State

        #region Draw

        [Header("Draw")]

        [SerializeField]
        [Range(0, 1)]
        [Tooltip("Scale when draw the card")]
        private float startSizeWhenDraw;

        public float StartSizeWhenDraw
        {
            get => startSizeWhenDraw;
            set => startSizeWhenDraw = value;
        }

        #endregion

        #region Hover

        [Header("Hover")]
        
        [SerializeField]
        [Range(0, 4)]
        [Tooltip("How much the card will go upwards when hovered.")]
        float hoverHeight;

        public float HoverHeight
        {
            get => hoverHeight;
            set => hoverHeight = value;
        }

        [SerializeField]
        [Range(0.9f, 2f)]
        [Tooltip("How much a hovered card scales.")]
        private float hoverScale;

        public float HoverScale
        {
            get => hoverScale;
            set => hoverScale = value;
        }

        [SerializeField]
        [Range(0, 25)]
        [Tooltip("How much the card will go upwards when hovered.")]
        private float hoverSpeed;

        public float HoverSpeed
        {
            get => hoverSpeed;
            set => hoverSpeed = value;
        }

        #endregion

        #endregion

        #region Motion

        #region Scale

        [Header("Scale")]

        [SerializeField]
        [Range(0, 15)]
        [Tooltip("Speed of a card while it is scaling")]
        private float scaleSpeed;

        public float ScaleSpeed
        {
            get => scaleSpeed;
            set => scaleSpeed = value;
        }

        #endregion

        #region Position

        [Header("Position")]

        [SerializeField]
        [Range(0, 15)]
        [Tooltip("Speed of a card while it is moving")]
        private float positionSpeed;

        public float PositionSpeed
        {
            get => positionSpeed;
            set => positionSpeed = value;
        }

        #endregion

        #region Rotation

        [Header("Rotation")]

        [SerializeField]
        [Range(0, 15)]
        [Tooltip("Speed of a card while it is rotating")]
        private float rotationSpeed;

        public float RotationSpeed
        {
            get => rotationSpeed;
            set => rotationSpeed = value;
        }
        #endregion

        #endregion
    }
}