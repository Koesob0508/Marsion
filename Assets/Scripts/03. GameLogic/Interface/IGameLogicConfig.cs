namespace Marsion
{
    /// <summary>
    ///     GameData의 기본 설정 값. Factory 패턴 응용해봄
    ///     (2024.12.20) 좋은 방법인지는 모르겠다...
    /// </summary>
    public interface IGameLogicConfig
    {
        int CountOfPlayer { get; }
        int MaxHealth { get; }
        int MaxMana { get; }
        int CountOfStartHand { get; }
    }

    public class DefaultGameLogicConfig : IGameLogicConfig
    {
        public int CountOfPlayer => 2;
        public int MaxHealth => 30;
        public int MaxMana => 0;
        public int CountOfStartHand => 3;
    }
}