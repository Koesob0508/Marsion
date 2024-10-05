using System;
using System.Collections.Generic;
using System.Linq;

namespace Marsion
{
    public class DraftState
    {
        public int Count;
        public List<Card> Deck;
        public List<Card> Selections;
        public List<Card> SubSelections;

        public DraftState()
        {
            Deck = new List<Card>();
            Selections = new List<Card>();
            SubSelections = new List<Card>();
        }
    }

    public class DraftServer
    {
        public enum SelectType
        {
            Legendary = 0,
            Table = 1,
            Exchange = 2
        }

        DraftState CurrentState;
        public DraftState State => CurrentState;

        public SelectType Type;
        public Queue<int> TypeSequence;
        public bool IsComplete;
        
        public Action OnUpdateDeckBuildingState;

        public void Init()
        {
            CurrentState = new DraftState();

            TypeSequence = new Queue<int>(Enumerable.Concat(
                new[] { 0 },
                Enumerable.Repeat(1, 29)
            ));

            CurrentState.Count = TypeSequence.Count;

            IsComplete = false;

            SetSelection();
        }

        public void SetSelection()
        {
            if (TypeSequence.TryDequeue(out var result))
            {
                Managers.Logger.Log<DraftServer>("Set Sequence", colorName: "yellow");
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

        public void Select(int index)
        {
            CurrentState.Count--;

            switch(Type)
            {
                case SelectType.Legendary:
                    CurrentState.Deck.Add(CurrentState.Selections[index]);
                    break;
                case SelectType.Table:
                    CurrentState.Deck.Add(CurrentState.Selections[index]);
                    break;
                case SelectType.Exchange:
                    CurrentState.Deck.Remove(CurrentState.SubSelections[index]);
                    CurrentState.Deck.Add(CurrentState.Selections[index]);
                    break;
            }

            SetSelection();

            OnUpdateDeckBuildingState?.Invoke();
        }

        public void Ready()
        {
            Managers.Client.Ready(CurrentState.Deck);
        }

        public void SetNextSelectSequence(Queue<int> sequence)
        {
            TypeSequence = sequence;
            CurrentState.Count = sequence.Count;
        }

        private void GenerateSelection()
        {
            switch (Type)
            {
                case SelectType.Legendary:
                    CurrentState.Selections = Managers.Card.FindByGrade(4, 3);
                    break;
                case SelectType.Table:
                    CurrentState.Selections = Managers.Card.FindExcludeGrade(4, 3);
                    break;
                case SelectType.Exchange:
                    CurrentState.SubSelections = Managers.Card.FindExcludeGrade(4, 3, cards: CurrentState.Deck);
                    CurrentState.Selections = Managers.Card.FindExcludeGrade(4, 3);
                    break;
            }
        }
    }
}