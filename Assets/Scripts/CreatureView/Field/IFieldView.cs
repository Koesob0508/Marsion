namespace Marsion.CardView
{
    public interface IFieldView
    {
        bool IsFullField { get; }
        int EmptyCreatureIndex { get; }
        void InsertEmptyCard(float x);
        void RemoveEmptyCard();
        void SpawnCard(Player player, Card card, int index);
        ICreatureView GetCreature(Card card);
    }
}