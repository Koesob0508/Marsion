namespace Marsion
{
    public interface IClientManager
    {
        IGameClient Game { get; }
        DraftClient Draft { get; }
        InputManager Input { get; }
        void Init();
    }
}