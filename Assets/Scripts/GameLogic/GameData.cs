using Marsion;
using Newtonsoft.Json;
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

        public GameData() { }

        public GameData(int playerCount)
        {
            Players = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
                Players[i] = new Player(i);
        }

        public Player GetPlayer(ulong clientID)
        {
            if (Players[clientID] == null)
            {
                Managers.Logger.LogWarning<GameData>("Get player result is null.");
                
                return null;
            }
            else
            {
                return Players[clientID];
            }
        }

        public Player GetOpponentPlayer(ulong clientID)
        {
            return GetPlayer(1 - clientID);
        }

        public Player GetOpponentPlayer(Player player)
        {
            return GetPlayer(1 - player.PlayerID);
        }

        public Card GetHandCard(ulong clientID, string cardUID)
        {
            return GetPlayer(clientID).GetCard(cardUID);
        }

        public Card GetFieldCard(ulong clientID, string cardUID)
        {
            return GetPlayer(clientID).GetCard(cardUID);
        }
    }
}