using UnityEngine;

namespace Marsion
{
    
    public class DeckView : MonoBehaviour, IDeckView
    {
        public CardView cardPrefab;
        public HandView hand;
        public GameObject EnemyDeck;
        public HandView EnemyHand;

        public void Init()
        {
            Managers.Client.OnDrawCard -= DrawCard;
            Managers.Client.OnDrawCard += DrawCard;
        }

        [Button]
        public void Draw()
        {
            Managers.Server.DrawButtonRpc(Managers.Client.ID);
        }

        public void DrawCard(ulong clientID, int count)
        {
            if (Managers.Client.ID == clientID)
            {
                for (int i = 0; i < count; i++)
                {
                    var cardObject = Instantiate(cardPrefab);
                    cardObject.IsMine = true;
                    cardObject.FrontImage.SetActive(true);
                    cardObject.BackImage.SetActive(false);
                    cardObject.transform.position = transform.position;
                    cardObject.name = $"Card_{i}";
                    hand.AddCard(cardObject);
                }
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var cardObject = Instantiate(cardPrefab);
                    cardObject.IsMine = false;
                    cardObject.FrontImage.SetActive(false);
                    cardObject.BackImage.SetActive(true);
                    cardObject.transform.position = EnemyDeck.transform.position;
                    cardObject.transform.rotation = EnemyDeck.transform.rotation;
                    cardObject.name = $"Enmey Card_{i}";
                    EnemyHand.AddCard(cardObject);
                }
            }
        }
    }
}