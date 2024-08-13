using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion.CardView
{
    public class HandView : MonoBehaviour
    {
        public bool IsMine;
        public List<ICardView> Cards { get; protected set; }
        public Action<ICardView[]> OnPileChanged;

        private HandBender Bender { get; set; }

        #region UnityCallbacks

        protected virtual void Awake()
        {
            Cards = new List<ICardView>();
            Clear();

            Bender = GetComponent<HandBender>();
        }

        private void Start()
        {
            Managers.Client.OnCardPlayed -= CardPlayed;
            Managers.Client.OnCardPlayed += CardPlayed;
        }

        private void Update()
        {
            Bender.Bend(Cards.ToArray());
        }

        #endregion

        #region Operations

        

        private void CardPlayed(ulong clientID, string uid)
        {
            foreach(ICardView card in Cards)
            {
                if(card.Card.UID == uid)
                {
                    Cards.Remove(card);
                    Managers.Resource.Destroy(card.MonoBehaviour.gameObject);
                    break;
                }
            }

            OnPileChanged?.Invoke(Cards.ToArray());
        }

        public void AddCard(ICardView card)
        {
            if (card == null)
                throw new ArgumentNullException("Null is not a valid argument");

            Cards.Add(card);

            for(int i = 0; i < Cards.Count; i++)
            {
                card?.Order.SetOriginOrder(i);
            }

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