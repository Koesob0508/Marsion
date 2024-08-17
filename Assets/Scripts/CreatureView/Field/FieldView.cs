using System.Collections.Generic;
using UnityEngine;

namespace Marsion.CardView
{
    [RequireComponent(typeof(Aligner))]
    public class FieldView : MonoBehaviour, IFieldView
    {
        [SerializeField] CreatureView EmptyCard;
        [SerializeField] CreatureView FieldCardPrefab;
        [SerializeField] List<CreatureView> FieldCards;

        const int MAX_CARD_COUNT = 7;

        [SerializeField] bool IsMine;
        bool IsExistEmptyCard => FieldCards.Exists(x => x == EmptyCard);
        public bool IsFullField => FieldCards.Count >= MAX_CARD_COUNT && !IsExistEmptyCard;
        public int EmptyCardIndex => FieldCards.FindIndex(x => x == EmptyCard);

        private Vector3 lastMousePosition;

        Aligner Aligner => GetComponent<Aligner>();

        private void Start()
        {
            Managers.Client.OnCardSpawned -= SpawnCard;
            Managers.Client.OnCardSpawned += SpawnCard;
        }

        public void InsertEmptyCard(float x)
        {
            if (IsFullField) return;

            if (!IsExistEmptyCard)
                FieldCards.Add(EmptyCard);

            Vector3 emptyCardPos = Vector3.zero;
            emptyCardPos.x = x;
            EmptyCard.transform.localPosition = emptyCardPos;

            int emptyCardIndex = EmptyCardIndex;
            FieldCards.Sort((card1, card2) => card1.transform.position.x.CompareTo(card2.transform.position.x));
            
            Aligner.Align(FieldCards.ToArray());
        }

        public void RemoveEmptyCard()
        {
            if (!IsExistEmptyCard) return;

            FieldCards.RemoveAt(EmptyCardIndex);
            Aligner.Align(FieldCards.ToArray());
        }

        public void SpawnCard(Player player, Card card, int index)
        {
            if (Managers.Client.IsMine(player) != IsMine) return;

            CreatureView fieldCard;

            if (EmptyCardIndex == index)
            {
                fieldCard = Instantiate(FieldCardPrefab, EmptyCard.transform.position, Quaternion.identity, transform);
                FieldCards[EmptyCardIndex] = fieldCard;
            }
            else
            {
                RemoveEmptyCard();
                fieldCard = Instantiate(FieldCardPrefab, EmptyCard.transform.position, Quaternion.identity, transform);
                FieldCards.Insert(index, fieldCard);
            }

            fieldCard.Setup(card);
            Aligner.Align(FieldCards.ToArray());
        }

        public void SetLastMousePosition(Vector3 position)
        {
            lastMousePosition = position;
        }
    }
}