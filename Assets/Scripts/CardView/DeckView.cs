using UnityEngine;

namespace Marsion.CardView
{
    
    public class DeckView : MonoBehaviour, IDeckView
    {
        public CardView cardPrefab;
        public HandView hand;
        public GameObject EnemyDeck;
        public HandView EnemyHand;

        public void Start()
        {
            Managers.Client.OnCardDrawn -= DrawCard;
            Managers.Client.OnCardDrawn += DrawCard;
        }

        [Button]
        public void Draw()
        {
            Managers.Server.DrawButtonRpc(Managers.Client.ID);
        }

        public void DrawCard(ulong clientID, string cardUID)
        {
            if (Managers.Client.ID == clientID)
            {
                var cardObject = Instantiate(cardPrefab);
                cardObject.Card = Managers.Client.GetCard(clientID, cardUID);
                cardObject.FrontImage.SetActive(true);
                cardObject.BackImage.SetActive(false);
                cardObject.transform.position = transform.position;
                cardObject.name = $"Card";
                hand.AddCard(cardObject);
            }
            else
            {
                var cardObject = Instantiate(cardPrefab);
                cardObject.Card = Managers.Client.GetCard(clientID, cardUID);
                cardObject.FrontImage.SetActive(false);
                cardObject.BackImage.SetActive(true);
                cardObject.transform.position = EnemyDeck.transform.position;
                cardObject.transform.rotation = EnemyDeck.transform.rotation;
                cardObject.name = $"Enmey Card";
                EnemyHand.AddCard(cardObject);
            }
        }
    }
}