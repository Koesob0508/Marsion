namespace Marsion
{
    public class GameCommand
    {
        public const ushort None = 0;

        // Client to Server
        public const ushort ClientUdpateData = 1000;

        // Server to Client
        public const ushort ServerUpdateData = 2000;
        public const ushort ServerStartGame = 2010;
    }
}