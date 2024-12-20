namespace Marsion
{
    /// <summary>
    ///     GameData의 기본 설정 값. Factory 패턴 응용해봄
    ///     (2024.12.20) 좋은 방법인지는 모르겠다...
    /// </summary>
    public interface IGameDataConfig
    {
        int CountOfPlayer { get; }
        int MaxHP { get; }
        int MaxMana { get; }
        int CountOfStartHand { get; }
    }

    public class DefaultGameDataConfig : IGameDataConfig
    {
        public int CountOfPlayer => 2;
        public int MaxHP => 30;
        public int MaxMana => 0;
        public int CountOfStartHand => 3;
    }
}