using System;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion.CardView
{
    public class HandView : MonoBehaviour, IHandView
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
            Managers.Client.Game.OnCardPlayed -= CardPlayed;
            Managers.Client.Game.OnCardPlayed += CardPlayed;

            Managers.Client.Game.OnGameReset -= ResetGame;
            Managers.Client.Game.OnGameReset += ResetGame;
        }

        private void Update()
        {
            Bender.Bend(Cards.ToArray());
        }

        #endregion

        #region Operations

        private void ResetGame()
        {
            foreach(var card in Cards)
            {
                Managers.Resource.Destroy(card.MonoBehaviour.gameObject);
            }

            Cards.Clear();
        }

        private void CardPlayed(bool succeeded, Player player, string cardUID)
        {
            if(succeeded)
            {
                foreach (ICardView cardView in Cards)
                {
                    if (cardView.Card.UID == cardUID)
                    {
                        Cards.Remove(cardView);
                        Managers.Resource.Destroy(cardView.MonoBehaviour.gameObject);
                        break;
                    }
                }

                OnPileChanged?.Invoke(Cards.ToArray());
            }
            else
            {
                foreach (ICardView cardView in Cards)
                {
                    if (cardView.Card.UID == cardUID)
                    {
                        cardView.MonoBehaviour.gameObject.SetActive(true);
                    }
                }
            }
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

        public void RemoveCard(ICardView card)
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