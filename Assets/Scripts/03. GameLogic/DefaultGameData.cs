using Newtonsoft.Json;
using System;

namespace Marsion
{
    /// <summary>
    ///     현재 플레이어, 턴의 진행 관리
    /// </summary>
    [Serializable]
    public class DefaultGameData : IGameData
    {
        public Player[] Players { get; private set; }
        public Player CurrentPlayer { get; set; }
        public int TurnCount { get; private set; }

        public void Init(IGameDataConfig config)
        {
            Players = new Player[config.CountOfPlayer];

            for (int i = 0; i < config.CountOfPlayer; i++)
            {
                Players[i] = new Player();
                Players[i].Init((ulong)i);
            }

            foreach(var player in Players)
            {
                player.SetMaxHP(config.MaxHP);
                player.SetMaxMana(config.MaxMana);
            }

            // Player 외적 설정
            TurnCount = 0;
            Random random = new Random();
            ulong firstPlayerID = (ulong)random.Next(0, 2);
            CurrentPlayer = GetPlayer(firstPlayerID);
        }

        public Player GetPlayer(ulong playerID)
        {
            if (Players[playerID] == null)
            {
                Logger.LogWarning<DefaultGameData>("Get player result is null.");

                return null;
            }
            else
            {
                return Players[playerID];
            }
        }

        public Card GetHandCard(ulong playerID, string cardUID)
        {
            if (GetPlayer(playerID).TryGetHandCard(cardUID, out Card card))
            {
                return card;
            }

            Logger.LogWarning<DefaultGameData>("Get hand card result is null.");
            return null;
        }

        public Card GetFieldCard(ulong playerID, string cardUID)
        {
            if (GetPlayer(playerID).TryGetFieldCard(cardUID, out Card card))
            {
                return card;
            }

            Logger.LogWarning<DefaultGameData>("Get field card result is null.");
            return null;
        }

        public void AdvanceTurn()
        {
            TurnCount++;
        }

        public void ChangeCurrentPlayer()
        {
            CurrentPlayer = CurrentPlayer == GetPlayer(0) ? GetPlayer(1) : GetPlayer(0);
        }
    }
}