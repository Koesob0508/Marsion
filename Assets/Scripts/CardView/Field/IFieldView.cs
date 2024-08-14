namespace Marsion.CardView
{
    public interface IFieldView
    {
        bool IsFullField { get; }
        int EmptyCardIndex { get; }
        void InsertEmptyCard(float x);
        void RemoveEmptyCard();
        void SpawnCard(ulong clientID, string uid, int index);
    }
}