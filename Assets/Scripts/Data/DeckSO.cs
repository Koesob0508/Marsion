using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "DeckSO", menuName = "Marsion/DeckSO")]
    public class DeckSO : ScriptableObject
    {
        public string title;

        public CardSO[] cards;

        public DeckSO(List<CardSO> cardList)
        {
            cards = new CardSO[30];
            for (int i = 0; i < cards.Length; i++)
            {
                cards[i] = cardList[i];
            }
        }
    }
}