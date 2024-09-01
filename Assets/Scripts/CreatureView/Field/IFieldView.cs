﻿namespace Marsion.CardView
{
    public interface IFieldView
    {
        bool IsFullField { get; }
        int EmptyCreatureIndex { get; }
        void InsertEmptyCard(float x);
        void RemoveEmptyCard();
        ICreatureView GetCreature(Card card);
    }
}