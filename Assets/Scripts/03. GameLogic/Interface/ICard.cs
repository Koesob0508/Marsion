namespace Marsion
{
    public interface ICard
    {
        string SOID { get; }
        string UID { get; }
        ulong PlayerID { get; }
        string Name { get; }
        int Power { get; }
        int Health { get; }
        bool IsDead { get; }

        // TODO 아래 함수 리팩토링
        void Init(ulong playerID);
        void SetMaxHealth(int amount);
    }
}