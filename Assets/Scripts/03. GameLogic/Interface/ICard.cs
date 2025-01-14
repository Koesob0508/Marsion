namespace Marsion
{
    public interface ICard
    {
        string SOID { get; }
        string UID { get; }
        ulong PlayerID { get; }
        string Name { get; }
        int Power { get; }
    }
}