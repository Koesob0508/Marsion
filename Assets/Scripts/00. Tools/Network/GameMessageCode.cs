namespace Marsion
{
    public class GameMessageCode
    {
        public const ushort None = 0;

        // Client to Server
        public const ushort ClientUdpateData = 1000;
        public const ushort ClientTrySpawnCard = 1010;
        public const ushort ClientTurnEnd = 1020;
        public const ushort ClientTryAttack = 1030;

        // Server to Client
        public const ushort ServerUpdateData = 2000;
        public const ushort ServerStartGame = 2010;
        public const ushort ServerEndGame = 2100;
        public const ushort ServerChangeMana = 2020;
        public const ushort ServerStartTurn = 2030;
        public const ushort ServerEndTurn = 2040;
        public const ushort ServerDrawCard = 2050;
        public const ushort ServerPlayCardResult = 2060;
        public const ushort ServerSpawnCardResult = 2070;
        public const ushort ServerAttackCardResult = 2080;
        public const ushort ServerDeadCards = 2090;
    }
}