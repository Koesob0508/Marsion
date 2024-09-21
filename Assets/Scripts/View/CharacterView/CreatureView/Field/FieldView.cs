using System.Collections.Generic;
using UnityEngine;

namespace Marsion.CardView
{
    [RequireComponent(typeof(Sorter))]
    public class FieldView : MonoBehaviour, IFieldView
    {
        [SerializeField] CreatureView EmptyCreaturePrefab;
        [SerializeField] CreatureView CreatureViewPrefab;

        const int MAX_CARD_COUNT = 7;

        [SerializeField] bool IsMine;

        CreatureView EmptyCreature;
        public List<ICreatureView> Creatures;
        bool IsExistEmptyCard => Creatures.Exists(x => (Object)x == EmptyCreature);
        public bool IsFullField => Creatures.Count >= MAX_CARD_COUNT && !IsExistEmptyCard;
        public int EmptyCreatureIndex => Creatures.FindIndex(x => (Object)x == EmptyCreature);

        Sorter Sorter => GetComponent<Sorter>();

        private void Start()
        {
            Managers.Client.OnCardSpawned -= SpawnCard;
            Managers.Client.OnCardSpawned += SpawnCard;

            Creatures = new List<ICreatureView>();

            EmptyCreature = Instantiate(EmptyCreaturePrefab, transform);
            EmptyCreature.Init(null);
        }

        private void Update()
        {
            Sorter.Sort(Creatures.ToArray());
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
        }

        public void RemoveEmptyCard()
        {
            if (!IsExistEmptyCard) return;

            Creatures.RemoveAt(EmptyCreatureIndex);
        }

        private void SpawnCard(bool succeeded, Player player, Card card, int index)
        {
            if (Managers.Client.IsMine(player) != IsMine) return;

            if(succeeded)
            {
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

                creature.Init(card);
                creature.Spawn();
            }
            else
            {
                RemoveEmptyCard();
            }
        }

        private void RemoveDeadCreature()
        {
            List<ICreatureView> removeObjects = new List<ICreatureView>();

            foreach(var creature in Creatures)
            {
                if(creature.Card.IsDead)
                {
                    creature.Clear();
                    removeObjects.Add(creature);
                }
            }

            foreach(var obj in removeObjects)
            {
                Creatures.Remove(obj);
                Managers.Resource.Destroy(obj.MonoBehaviour.gameObject);
            }

            removeObjects.Clear();
        }

        public ICharacterView GetCreature(Card card)
        {
            return Creatures.Find(x => x.Card.UID == card.UID);
        }
    }
}