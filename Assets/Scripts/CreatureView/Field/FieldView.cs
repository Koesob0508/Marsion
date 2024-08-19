using System.Collections.Generic;
using UnityEngine;

namespace Marsion.CardView
{
    [RequireComponent(typeof(Aligner))]
    public class FieldView : MonoBehaviour, IFieldView
    {
        [SerializeField] CreatureView EmptyCreature;
        [SerializeField] CreatureView CreatureViewPrefab;
        [SerializeField] List<ICreatureView> Creatures;

        const int MAX_CARD_COUNT = 7;

        [SerializeField] bool IsMine;
        bool IsExistEmptyCard => Creatures.Exists(x => (Object)x == EmptyCreature);
        public bool IsFullField => Creatures.Count >= MAX_CARD_COUNT && !IsExistEmptyCard;
        public int EmptyCreatureIndex => Creatures.FindIndex(x => (Object)x == EmptyCreature);

        private Vector3 lastMousePosition;

        Aligner Aligner => GetComponent<Aligner>();

        private void Start()
        {
            Managers.Client.OnCardSpawned -= SpawnCard;
            Managers.Client.OnCardSpawned += SpawnCard;

            Creatures = new List<ICreatureView>();
        }

        public void InsertEmptyCard(float x)
        {
            if (IsFullField) return;

            if (!IsExistEmptyCard)
                Creatures.Add(EmptyCreature);

            Vector3 emptyCreaturePos = Vector3.zero;
            emptyCreaturePos.x = x;
            EmptyCreature.Transform.localPosition = emptyCreaturePos;

            int emptyCardIndex = EmptyCreatureIndex;
            Creatures.Sort((creature1, creature2) => creature1.Transform.position.x.CompareTo(creature2.Transform.position.x));
            
            Aligner.Align(Creatures.ToArray());
        }

        public void RemoveEmptyCard()
        {
            if (!IsExistEmptyCard) return;

            Creatures.RemoveAt(EmptyCreatureIndex);
            Aligner.Align(Creatures.ToArray());
        }

        public void SpawnCard(Player player, Card card, int index)
        {
            if (Managers.Client.IsMine(player) != IsMine) return;

            ICreatureView creature;

            if (EmptyCreatureIndex == index)
            {
                creature = Instantiate(CreatureViewPrefab, EmptyCreature.Transform.position, Quaternion.identity, transform);
                Creatures[EmptyCreatureIndex] = creature;
            }
            else
            {
                RemoveEmptyCard();
                creature = Instantiate(CreatureViewPrefab, EmptyCreature.Transform.position, Quaternion.identity, transform);
                Creatures.Insert(index, creature);
            }

            creature.Setup(card);
            Aligner.Align(Creatures.ToArray());
            creature.Spawn();
        }

        public void SetLastMousePosition(Vector3 position)
        {
            lastMousePosition = position;
        }
    }
}