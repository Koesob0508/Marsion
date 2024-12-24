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

        public void Init(IGameLogicConfig config, List<PlayerInfo> playerInfos)
        {
            Players = new Player[config.CountOfPlayer];

            for (int i = 0; i < config.CountOfPlayer; i++)
            {
                Players[i] = new Player();
                Players[i].SetPlayerID((ulong)i);
                Players[i].SetPlayerPortrait(playerInfos[i].Portrait);
                Players[i].SetMaxHealth(config.MaxHealth);
                Players[i].SetMaxMana(config.MaxMana);

                var deck = new List<Card>();

                foreach(var soID in playerInfos[i].Deck)
                {
                    var card = new Card();
                    card.Init((ulong)i, soID);
                    deck.Add(card);
                }

                Players[i].SetDeck(deck);
                Players[i].Init();
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