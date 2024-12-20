using System;
using System.Collections.Generic;

namespace Marsion
{
    /// <summary>
    ///     GameData 간접 조작, Logic과 Data의 중간
    /// </summary>
    public class DefaultGameDataHandler : IGameDataHandler
    {
        public IGameData GameData { get; }

        public Player CurrentPlayer => GameData.CurrentPlayer;

        public DefaultGameDataHandler(IGameData gameData)
        {
            GameData = gameData;
        }

        public void Init()
        {
            var dataConfig = new DefaultGameDataConfig();
            GameData.Init(dataConfig);

            #region 초상화 설정. 추후 외부에서 등록하도록 바뀔 예정
            Random random = new Random();
            int number1 = random.Next(3, 13); // Next의 두 번째 인자는 상한을 포함하지 않으므로 13을 사용
            // 두 번째 숫자 뽑기 (첫 번째 숫자와 중복되지 않도록)
            int number2;
            do
            {
                number2 = random.Next(3, 13);
            } while (number2 == number1);

            #endregion

            SetPlayerPortrait(0, number1.ToString());
            SetPlayerPortrait(1, number2.ToString());

            foreach (var player in GameData.Players)
            {
                ShuffleDeck(player);
                DrawCard(player, out var mulliganTarget, dataConfig.CountOfStartHand);
            }
        }

        // 사전 작업
        private void SetPlayerPortrait(ulong playerID, string portraitID)
        {
            GameData.GetPlayer(playerID).SetPlayerPortrait(portraitID);
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

        public void SetPlayerDeck(ulong playerID, List<string> deck)
        {
            Logger.Log<DefaultGameDataHandler>($"Set player deck", colorName: ColorCodes.Logic);
            Player player = GetPlayer(playerID);
            List<Card> resultDeck = new List<Card>();

            foreach (var soID in deck)
            {
                if (Managers.Instance.Data.GetDictionary<CardSO>().TryGetValue(soID, out var cardSO))
                {
                    resultDeck.Add(new Card(playerID, cardSO));
                }
                else
                {
                    Logger.Log<DefaultGameDataHandler>($"{soID} CardSO not found", colorName: ColorCodes.Logic);
                }
            }

            player.Deck = resultDeck;
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
