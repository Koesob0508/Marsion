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

        public void DrawCard(Player player, Card card)
        {
            if (Managers.Client.IsMine(player))
            {
                var cardObject = Instantiate(cardPrefab);
                Managers.Logger.Log<DeckView>($"UID : {card.UID}");
                cardObject.Card = card;
                cardObject.FrontImage.SetActive(true);
                cardObject.BackImage.SetActive(false);
                cardObject.transform.position = transform.position;
                cardObject.name = $"Card";
                cardObject.Setup();
                hand.AddCard(cardObject);
            }
            else
            {
                var cardObject = Instantiate(cardPrefab);
                cardObject.Card = card;
                cardObject.FrontImage.SetActive(false);
                cardObject.BackImage.SetActive(true);
                cardObject.transform.position = EnemyDeck.transform.position;
                cardObject.transform.rotation = EnemyDeck.transform.rotation;
                cardObject.name = $"Enmey Card";
                cardObject.Setup();
                EnemyHand.AddCard(cardObject);
            }
        }
    }
}