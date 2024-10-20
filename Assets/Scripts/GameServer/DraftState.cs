using System;
using System.Collections.Generic;

namespace Marsion
{
    public class DraftState
    {
        public bool IsComplete;
        public int Count;
        public List<string> CurrentDeck;
        public List<string> CurrentSelections;
        public List<string> CurrentSubSelections;

        public SelectType Type;
        public Queue<int> TypeSequence;

        public DraftState()
        {
            CurrentDeck = new List<string>();
            CurrentSelections = new List<string>();
            CurrentSubSelections = new List<string>();
        }

        public DraftState(Queue<int> sequence)
        {
            IsComplete = false;
            CurrentDeck = new();
            CurrentSelections = new();
            CurrentSubSelections = new();
            TypeSequence = new Queue<int>(sequence);
            Count = TypeSequence.Count;
        }

        public DraftState(SerializedDraftState serialized)
        {
            CurrentDeck = new List<string>();
            CurrentSelections = new List<string>();
            CurrentSubSelections = new List<string>();

            IsComplete = serialized.isComplete;
            Count = serialized.count;

            foreach (var cardID in serialized.deck)
            {
                CurrentDeck.Add(cardID);
            }

            foreach (var cardID in serialized.selections)
            {
                CurrentSelections.Add(cardID);
            }

            foreach (var cardID in serialized.subSelections)
            {
                CurrentSubSelections.Add(cardID);
            }
        }

        public void Select(int index)
        {
            Count--;

            switch (Type)
            {
                case SelectType.Legendary:
                    CurrentDeck.Add(CurrentSelections[index]);
                    break;
                case SelectType.Table:
                    CurrentDeck.Add(CurrentSelections[index]);
                    break;
                case SelectType.Exchange:
                    CurrentDeck.Remove(CurrentSubSelections[index]);
                    CurrentDeck.Add(CurrentSelections[index]);
                    break;
            }

            SetSelection();
        }

        public void SetSelection()
        {
            if (TypeSequence.TryDequeue(out var result))
            {
                SelectType[] types = (SelectType[])Enum.GetValues(typeof(SelectType));
                Type = types[result];

                GenerateSelection();

                IsComplete = false;
            }
            else
            {
                TypeSequence.Clear();

                IsComplete = true;
            }
        }

        private void GenerateSelection()
        {
            switch (Type)
            {
                case SelectType.Legendary:
                    CurrentSelections = Managers.Card.FindByGrade(4, 3);
                    break;
                case SelectType.Table:
                    CurrentSelections = Managers.Card.FindExcludeGrade(4, 3);
                    break;
                case SelectType.Exchange:
                    CurrentSubSelections = Managers.Card.FindExcludeGrade(4, 3, pile: CurrentDeck);
                    CurrentSelections = Managers.Card.FindExcludeGrade(4, 3);
                    break;
            }
        }
    }
}