namespace Marsion
{
    public interface IDeckView
    {
        void Init();
        void DrawCard(ulong clientID, int count);
    }
}