namespace Marsion
{
    public static class DraftCommand
    {
        public const ushort None = 0;

        // Client to Server
        public const ushort ClientStartDraft = 1010;
        public const ushort ClientSelect = 1020;
        public const ushort ClientReady = 1030;

        // Servr to Client
        public const ushort ServerInitState = 2000;
        public const ushort ServerStartDraft = 2010;
        public const ushort ServerUpdateState = 2020;
    }
}