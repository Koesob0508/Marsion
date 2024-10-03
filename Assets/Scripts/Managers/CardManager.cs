using System.Collections.Generic;
using System;
using UnityEngine;

namespace Marsion
{
    public class CardManager : MonoBehaviour
    {
        public List<CardSO> Cards;

        public List<Card> FindByGrade(int grade, int count = 1, bool allowDuplicate = false, List<Card> cards = null)
        {
            List<Card> result = new List<Card>();
            List<Card> copiedCards;

            if (cards == null)
            {
                copiedCards = new List<Card>();

                foreach(var cardSO in Cards)
                {
                    copiedCards.Add(new Card(Managers.Client.ID, cardSO));
                }
            }
            else
            {
                copiedCards = new List<Card>(cards);
            }

            ShuffleList(copiedCards);

            while (result.Count < count)
            {
                Card selected = null;

                foreach (var card in copiedCards)
                {
                    if ((int)card.Grade == grade)
                    {
                        selected = card;
                        result.Add(selected);
                        break;
                    }
                }

                if (selected == null)
                    break;

                if (!allowDuplicate)
                {
                    copiedCards.Remove(selected);
                }
            }

            return result;
        }

        public List<Card> FindExcludeGrade(int grade, int count = 1, bool allowDuplicate = false, List<Card> cards = null)
        {
            List<Card> result = new List<Card>();
            List<Card> copiedCards;

            if (cards == null)
            {
                copiedCards = new List<Card>();

                foreach (var cardSO in Cards)
                {
                    copiedCards.Add(new Card(Managers.Client.ID, cardSO));
                }
            }
            else
            {
                copiedCards = new List<Card>(cards);
            }

            ShuffleList(copiedCards);

            while (result.Count < count)
            {
                Card selected = null;

                foreach (var card in copiedCards)
                {
                    if ((int)card.Grade != grade)
                    {
                        selected = card;
                        result.Add(selected);
                        break;
                    }
                }

                if (selected == null)
                    break;

                if (!allowDuplicate)
                {
                    copiedCards.Remove(selected);
                }
            }

            return result;
        }

        private void ShuffleList<T>(List<T> list)
        {
            System.Random rng = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}