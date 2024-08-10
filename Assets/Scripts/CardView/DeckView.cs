using UnityEngine;

namespace Marsion
{
    
    public class DeckView : MonoBehaviour, IDeckView
    {
        public CardView cardPrefab;
        public HandView hand;

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
            if (Managers.Client.ID != clientID) return;

            for(int i = 0; i < count; i++)
            {
                var cardObject = Instantiate(cardPrefab);
                cardObject.transform.position = transform.position;
                cardObject.name = $"Card_{i}";
                hand.AddCard(cardObject);
            }
        }
    }
}