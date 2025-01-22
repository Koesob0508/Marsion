namespace Marsion
{
    public struct PlayerAndCard
    {
        public ulong PlayerID;
        public string CardUID; 
    }
    public class EventData
    {
        public EventType Type;
        public PlayerAndCard Commander;
        public PlayerAndCard Target;
    }
}