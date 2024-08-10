using Marsion;
using System;

namespace Marsion
{
    [Serializable]
    public class GameData
    {
        public Player[] Players;

        public int firstPlayer = 0;
        public int currentPlayer = 0;
        public int turnCount = 0;

        public GameData(int playerCount)
        {
            Players = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
                Players[i] = new Player(i);
        }
    }
}