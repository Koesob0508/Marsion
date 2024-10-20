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

        [JsonConstructor]
        public GameData()
        {

        }

        public GameData(int playerCount)
        {
            Players = new Player[playerCount];

            for (int i = 0; i < playerCount; i++)
                Players[i] = new Player(i);
        }

        public GameData(GameData original)
        {
            // Players 배열 깊은 복사
            if (original.Players != null)
            {
                Players = new Player[original.Players.Length];
                for (int i = 0; i < original.Players.Length; i++)
                {
                    Players[i] = new Player(original.Players[i]); // Player 클래스에서 복사 생성자를 사용
                }
            }

            // 현재 플레이어와 턴 카운트 복사
            CurrentPlayer = original.CurrentPlayer; // Player 객체가 참조형이면 깊은 복사를 원할 수 있음
            TurnCount = original.TurnCount;
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
            return GetPlayer(1 - player.ClientID);
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