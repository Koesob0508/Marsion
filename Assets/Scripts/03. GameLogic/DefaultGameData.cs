using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Marsion
{
    /// <summary>
    ///     현재 플레이어, 턴의 진행 관리
    /// </summary>
    [Serializable]
    public class DefaultGameData : IGameData
    {
        [JsonProperty] public Player[] Players { get; private set; }
        [JsonProperty] public Player CurrentPlayer { get; set; }
        [JsonProperty] public int TurnCount { get; private set; }

        public void Init(IGameLogicConfig config)
        {
            // Player 외적 설정
            TurnCount = 0;

            Players = new Player[config.CountOfPlayer];
        }

        public void SetPlayer(int index, Player player)
        {
            if(index >= Players.Length)
            {
                Logger.LogError<IGameData>($"Index out of range. Received index : {index}. Current Players length : {Players.Length}", colorName: ColorCodes.Logic);
                
                return;
            }

            Players[index] = player;
        }

        public void SetCurrentPlayer(ulong playerID)
        {
            CurrentPlayer = GetPlayer(playerID);
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
            if (GetPlayer(playerID).TryGetPlayerCard(cardUID, out var playerCard))
            {
                return playerCard;
            }

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