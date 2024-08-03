namespace Marsion
{
    public class GameData
    {
        public Player[] players;

        public int firstPlayer = 0;
        public int currentPlayer = 0;
        public int turnCount = 0;

        public GameData(int playerCount)
        {
            players = new Player[playerCount];
            for(int i = 0; i < playerCount; i++)
            {
                players[i] = new Player();
            }
        }
    }
}