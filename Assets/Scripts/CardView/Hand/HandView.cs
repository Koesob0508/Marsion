using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class HandView : MonoBehaviour
    {
        public List<ICardView> Cards { get; protected set; }
        private event Action<ICardView[]> onPileChanged = hand => { };
        public Action<ICardView[]> OnPileChanged
        {
            get => onPileChanged;
            set => onPileChanged = value;
        }

        private HandBender Bender { get; set; }

        #region UnityCallbacks

        protected virtual void Awake()
        {
            Cards = new List<ICardView>();
            Clear();

            Bender = GetComponent<HandBender>();
        }

        private void Update()
        {
            Bender.Bend(Cards.ToArray());
        }

        #endregion

        #region Operations

        public void AddCard(ICardView card)
        {
            if (card == null)
                throw new ArgumentNullException("Null is not a valid argument");

            Cards.Add(card);

            OnPileChanged?.Invoke(Cards.ToArray());

            card.Transform.SetParent(transform);
            card.Draw();
        }

        public  void RemoveCard(ICardView card)
        {
            if (card == null)
                throw new ArgumentNullException("Null is not a valid argument");

            Cards.Remove(card);

            OnPileChanged?.Invoke(Cards.ToArray());
        }

        private void Clear()
        {
            Cards.Clear();
        }

        #endregion
    }
}