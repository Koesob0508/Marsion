namespace Marsion
{
    public class GameCommand
    {
        public const ushort None = 0;

        // Client to Server
        public const ushort ClientUdpateData = 1000;
        public const ushort ClientTrySpawnCard = 1010;

        // Server to Client
        public const ushort ServerUpdateData = 2000;
        public const ushort ServerStartGame = 2010;
        public const ushort ServerChangeMana = 2020;
        public const ushort ServerStartTurn = 2030;
        public const ushort ServerDrawCard = 2040;
        public const ushort ServerPlayCardResult = 2050;
        public const ushort ServerSpawnCardResult = 2060;
    }
}