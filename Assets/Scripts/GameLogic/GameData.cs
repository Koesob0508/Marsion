using Marsion;
using System;
using System.Collections.Generic;

namespace Marsion.Logic
{
    [Serializable]
    public class GameData
    {
        public Player[] Players;
        public Player CurrentPlayer;
        public int TurnCount = 0;

        public GameData(int playerCount)
        {
            Players = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
                Players[i] = new Player(i);
        }
        
        public Player GetPlayer(ulong clientID)
        {
            return Players[clientID];
        }

        public Card GetHandCard(ulong clientID, string cardUID)
        {
            foreach (Card card in GetPlayer(clientID).Hand)
            {
                if (card.UID == cardUID)
                {
                    return card;
                }
            }

            Managers.Logger.Log<GameData>("Hand card is null.");

            return null;
        }

        public Card GetFieldCard(ulong clientID, string cardUID)
        {
            foreach (Card card in GetPlayer(clientID).Field)
            {
                if (card.UID == cardUID)
                {
                    return card;
                }
            }

            Managers.Logger.Log<GameData>("Field card is null.");

            return null;
        }
    }
}