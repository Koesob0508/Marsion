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

        public DefaultPlayer CurrentPlayer => GameData.CurrentPlayer;

        public void Init(IGameDataHandlerFactory dataHandlerFactory)
        {
            Logic = dataHandlerFactory.ProvideGameLogic();

            GameData = dataHandlerFactory.CreateGameData();
            GameData.Init(dataHandlerFactory.ProvideGameLogicConfig());

            var cardSOs = Logic.Data.GetDictionary<CardSO>();
            var config = dataHandlerFactory.ProvideGameLogicConfig();
            var playerInfos = dataHandlerFactory.ProvidePlayerInfos();

            foreach (var playerInfo in playerInfos)
            {
                var player = new DefaultPlayer();

                player.SetPlayerID(playerInfo.ClientID);
                player.SetPlayerPortrait(playerInfo.Portrait);
                player.SetMaxHealth(config.MaxHealth);
                player.SetMaxMana(config.MaxMana);

                var deck = new List<ICard>();
                foreach (var soID in playerInfo.Deck)
                {
                    var card = new Card();

                    if (cardSOs.TryGetValue(soID, out var cardSO))
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

            if (playerInfos.Count == 0)
            {
                return;
            }

            Random random = new Random();
            int firstPlayerIndex = random.Next(0, playerInfos.Count);
            ulong firstPlayerID = playerInfos[firstPlayerIndex].ClientID;

            GameData.SetCurrentPlayer(firstPlayerID);
        }

        public bool TryGetPlayer(ulong playerID, out DefaultPlayer player)
        {
            player = GameData.GetPlayer(playerID);

            if (player == null)
            {
                Logger.Log<IGameDataHandler>("Player ID not found.", colorName: ColorCodes.Logic);
                return false;
            }
            return true;
        }

        public ulong GetOpponentPlayerID(ulong playerID)
        {
            foreach (var opponentID in GameData.Players.Keys)
            {
                if (opponentID != playerID)
                {
                    return opponentID;
                }
            }

            throw new Exception("Other player ID not found.");
        }

        public ICard GetCardFromHand(ulong playerID, string cardUID)
        {
            if (!TryGetPlayer(playerID, out var player)) return null;

            if (!player.TryGetHandCard(cardUID, out var card))
            {
                Logger.Log<IGameDataHandler>("Card UID not found.", colorName: ColorCodes.Logic);
                return null;
            }

            return card;
        }

        public ICard GetCardFromField(ulong playerID, string cardUID)
        {
            if (!TryGetPlayer(playerID, out var player)) return null;

            if (player.TryGetPlayerCard(cardUID, out var playerCard))
            {
                return playerCard;
            }

            if (player.TryGetFieldCard(cardUID, out var card))
            {
                return card;
            }

            Logger.Log<IGameDataHandler>("Card UID not found.", colorName: ColorCodes.Logic);
            return null;
        }

        public void PayMana(ulong playerID, int amount)
        {
            if (!TryGetPlayer(playerID, out var player)) return;
            player.PayMana(amount);
        }

        public void AddCardToField(ulong playerID, ICard card, int index)
        {
            if (!TryGetPlayer(playerID, out var player)) return;
            player.Field.Insert(index, card);
        }

        public void AddCardToHand(ulong playerID, ICard card)
        {
            if (!TryGetPlayer(playerID, out var player)) return;
            player.Hand.Add(card);
        }

        public void RemoveCardFromField(ulong playerID, string cardUID)
        {
            if (!TryGetPlayer(playerID, out var player)) return;
            var card = GetCardFromField(playerID, cardUID);
            player.Field.Remove(card);
        }

        public void RemoveCardFromHand(ulong playerID, string cardUID)
        {
            if (!TryGetPlayer(playerID, out var player)) return;
            var card = GetCardFromHand(playerID, cardUID);
            player.Hand.Remove(card);
        }

        public void ShuffleDeck(ulong playerID)
        {
            Logger.Log<DefaultGameDataHandler>("Shuffle deck", colorName: ColorCodes.Logic);
            if (!TryGetPlayer(playerID, out var player)) return;
            List<ICard> deck = player.Deck;

            Random rng = new Random();
            int n = deck.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                ICard value = deck[k];
                deck[k] = deck[n];
                deck[n] = value;
            }
        }

        public void DrawCard(ulong playerID, out ICard drawnCard)
        {
            Logger.Log<DefaultGameDataHandler>("Draw a card", colorName: ColorCodes.Logic);
            if (!TryGetPlayer(playerID, out var player))
            {
                drawnCard = null;
                return;
            }

            ICard card = null;

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

        public void DrawCard(ulong playerID, out List<ICard> drawnCards, int count = 1)
        {
            Logger.Log<DefaultGameDataHandler>("Draw cards", colorName: ColorCodes.Logic);
            if (!TryGetPlayer(playerID, out var player))
            {
                drawnCards = null;
                return;
            }

            List<ICard> outCards = new();
            ICard card = null;

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
