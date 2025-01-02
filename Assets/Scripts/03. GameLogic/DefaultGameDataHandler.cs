using System;
using System.Collections.Generic;

namespace Marsion
{
    /// <summary>
    ///     GameData 간접 조작, Logic과 Data의 중간
    /// </summary>
    public class DefaultGameDataHandler : IGameDataHandler
    {
        public IGameLogic Logic { get; private set; }
        public IGameData GameData { get; private set; }

        public Player CurrentPlayer => GameData.CurrentPlayer;

        public void Init(IGameDataHandlerFactory dataHandlerFactory)
        {
            Logic = dataHandlerFactory.ProvideGameLogic();

            GameData = dataHandlerFactory.CreateGameData();
            GameData.Init(dataHandlerFactory.ProvideGameLogicConfig());

            var cardSOs = Logic.Data.GetDictionary<CardSO>();
            var config = dataHandlerFactory.ProvideGameLogicConfig();
            var playerInfos = dataHandlerFactory.ProvidePlayerInfos();

            foreach(var playerInfo in playerInfos)
            {
                var player = new Player();

                player.SetPlayerID(playerInfo.ClientID);
                player.SetPlayerPortrait(playerInfo.Portrait);
                player.SetMaxHealth(config.MaxHealth);
                player.SetMaxMana(config.MaxMana);

                var deck = new List<Card>();
                foreach(var soID in playerInfo.Deck)
                {
                    var card = new Card();

                    if(cardSOs.TryGetValue(soID, out var cardSO))
                    {
                        card.Init(Logic, playerInfo.ClientID, cardSO);
                        deck.Add(card);
                    }
                    else
                    {
                        Logger.LogWarning<Card>($"The CardSO ID {soID} was not found.", colorName: ColorCodes.Logic);
                    }
                }

                player.SetDeck(deck);
                player.Init();

                GameData.SetPlayer(player);
            }

            if(playerInfos.Count == 0)
            {
                return;
            }

            Random random = new Random();
            int firstPlayerIndex = random.Next(0, playerInfos.Count);
            ulong firstPlayerID = playerInfos[firstPlayerIndex].ClientID;

            GameData.SetCurrentPlayer(firstPlayerID);
        }

        public Player GetPlayer(ulong playerID) => GameData.GetPlayer(playerID);

        public ulong GetOpponentPlayer(ulong playerID)
        {
            foreach(var opponentID in GameData.Players.Keys)
            {
                if(opponentID != playerID)
                {
                    return GetPlayer(opponentID).PlayerID;
                }
            }

            throw new Exception("Other player ID not found.");
        }

        public Card GetCardFromHand(ulong playerID, string cardUID)
        {
            if (GetPlayer(playerID).TryGetHandCard(cardUID, out var card))
            {
                return card;
            }

            return null;
        }

        public Card GetCardFromField(ulong playerID, string cardUID)
        {
            if (GetPlayer(playerID).TryGetPlayerCard(cardUID, out var playerCard))
            {
                return playerCard;
            }

            if (GetPlayer(playerID).TryGetFieldCard(cardUID, out var card))
            {
                return card;
            }

            return null;
        }

        public void AddCardToField(ulong playerID, Card card, int index)
        {
            GetPlayer(playerID).Field.Insert(index, card);
        }

        public void AddCardToHand(ulong playerID, Card card)
        {
            GetPlayer(playerID).Hand.Add(card);
        }

        public void RemoveCardFromField(ulong playerID, string cardUID)
        {
            var card = GetCardFromField(playerID, cardUID);
            GetPlayer(playerID).Field.Remove(card);
        }

        public void RemoveCardFromHand(ulong playerID, string cardUID)
        {
            var card = GetCardFromHand(playerID, cardUID);
            GetPlayer(playerID).Hand.Remove(card);
        }

        public void ShuffleDeck(ulong playerID)
        {
            Logger.Log<DefaultGameDataHandler>("Shuffle deck", colorName: ColorCodes.Logic);
            List<Card> deck = GetPlayer(playerID).Deck;

            Random rng = new Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                Card value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        public void DrawCard(ulong playerID, out Card drawnCard)
        {
            Logger.Log<DefaultGameDataHandler>("Draw a card", colorName: ColorCodes.Logic);

            Card card = null;

            if (GetPlayer(playerID).Deck.Count > 0 && GetPlayer(playerID).Hand.Count < 10)
            {
                card = GetPlayer(playerID).Deck[0];
                GetPlayer(playerID).Deck.RemoveAt(0);
                GetPlayer(playerID).Hand.Add(card);
            }
            else
            {
                Logger.LogWarning<DefaultGameDataHandler>("Can't draw", colorName: ColorCodes.Logic);
            }

            drawnCard = card;
        }

        public void DrawCard(ulong playerID, out List<Card> drawnCards, int count = 1)
        {
            Logger.Log<DefaultGameDataHandler>("Draw cards", colorName: ColorCodes.Logic);

            List<Card> outCards = new();

            Card card = null;

            for (int i = 0; i < count; i++)
            {
                if (GetPlayer(playerID).Deck.Count > 0 && GetPlayer(playerID).Hand.Count < 10)
                {
                    card = GetPlayer(playerID).Deck[0];
                    GetPlayer(playerID).Deck.RemoveAt(0);
                    GetPlayer(playerID).Hand.Add(card);

                    outCards.Add(card);
                }
                else
                {
                    Logger.LogWarning<DefaultGameDataHandler>("Can't draw", colorName: ColorCodes.Logic);
                }
            }

            drawnCards = outCards;
        }

        public void AdvanceTurn() => GameData.AdvanceTurn();

        public void ChangeCurrentPlayer() => GameData.ChangeCurrentPlayer();
    }
}
