using System.Collections.Generic;
using System;
using UnityEngine;

namespace Marsion
{
    public class CardManager
    {
        public List<string> FindByGrade(int grade, int count = 1, bool allowDuplicate = false, List<string> cards = null)
        {
            List<string> result = new();
            List<string> copiedCards;

            if (cards == null)
            {
                copiedCards = new();

                foreach (var cardData in Managers.Instance.Resource.GetDictionary<CardSO>().Values)
                {
                    copiedCards.Add(cardData.ID);
                }
            }
            else
            {
                copiedCards = new();
            }

            ShuffleList(copiedCards);

            while (result.Count < count)
            {
                string selected = null;

                foreach (var cardID in copiedCards)
                {
                    if(Managers.Instance.Resource.GetDictionary<CardSO>().TryGetValue(cardID, out var cardSO))
                    {
                        if ((int)cardSO.Grade == grade)
                        {
                            selected = cardID;
                            result.Add(selected);
                            break;
                        }
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

        public List<string> FindExcludeGrade(int grade, int count = 1, bool allowDuplicate = false, List<string> pile = null)
        {
            List<string> result = new List<string>();
            List<string> copiedCards;

            if (pile == null)
            {
                copiedCards = new List<string>();

                foreach (var cardSO in Managers.Instance.Resource.GetDictionary<CardSO>().Values)
                {
                    copiedCards.Add(cardSO.ID);
                }
            }
            else
            {
                copiedCards = new List<string>(pile);
            }

            ShuffleList(copiedCards);

            while (result.Count < count)
            {
                string selected = null;

                foreach (var cardID in copiedCards)
                {
                    if(Managers.Instance.Resource.GetDictionary<CardSO>().TryGetValue(cardID, out var card))
                    {
                        if ((int)card.Grade != grade)
                        {
                            selected = cardID;
                            result.Add(selected);
                            break;
                        }
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