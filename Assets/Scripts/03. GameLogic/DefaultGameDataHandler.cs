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

                GameData.SetPlayer((int)playerInfo.ClientID, player);
            }

            if(playerInfos.Count == 0)
            {
                return;
            }

            Random random = new Random();
            ulong firstPlayerID = (ulong)random.Next(0, playerInfos.Count);
            GameData.SetCurrentPlayer(firstPlayerID);
        }

        public Player GetPlayer(ulong playerID) => GameData.GetPlayer(playerID);

        public Player GetOpponentPlayer(ulong playerID) => GameData.GetPlayer(1 - playerID);

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

        public void AddCardToField(ulong playerID, Card card)
        {
            throw new NotImplementedException();
        }

        public void AddCardToHand(ulong playerID, Card card)
        {
            throw new NotImplementedException();
        }

        public void RemoveCardFromField(ulong playerID, string cardUID)
        {
            throw new NotImplementedException();
        }

        public void RemoveCardFromHand(ulong playerID, string cardUID)
        {
            throw new NotImplementedException();
        }

        public void ShuffleDeck(Player player)
        {
            Logger.Log<DefaultGameDataHandler>("Shuffle deck", colorName: ColorCodes.Logic);
            List<Card> deck = player.Deck;

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

        public void DrawCard(Player player, out Card drawnCard)
        {
            Logger.Log<DefaultGameDataHandler>("Draw a card", colorName: ColorCodes.Logic);

            Card card = null;

            if (player.Deck.Count > 0 && player.Hand.Count < 10)
            {
                card = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(card);
            }
            else
            {
                Logger.LogWarning<DefaultGameDataHandler>("Can't draw", colorName: ColorCodes.Logic);
            }

            drawnCard = card;
        }

        public void DrawCard(Player player, out List<Card> drawnCards, int count = 1)
        {
            Logger.Log<DefaultGameDataHandler>("Draw cards", colorName: ColorCodes.Logic);

            List<Card> outCards = new();

            Card card = null;

            for (int i = 0; i < count; i++)
            {
                if (player.Deck.Count > 0 && player.Hand.Count < 10)
                {
                    card = player.Deck[0];
                    player.Deck.RemoveAt(0);
                    player.Hand.Add(card);

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
