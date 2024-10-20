using Marsion.Logic;
using System;
using System.Collections.Generic;

namespace Marsion
{
    public class GameLogicEx
    {
        public GameData Data;
        public event Action OnDataUpdated;
        public event Action OnGameStarted;

        public GameLogicEx(GameData data)
        {
            Data = data;
        }

        public void SetPlayerDeck(ulong clientID, List<string> deck)
        {
            Managers.Logger.Log<GameLogicEx>($"Set player deck", colorName: ColorCodes.Server);
            Player player = Data.GetPlayer(clientID);
            List<Card> resultDeck = new List<Card>();

            foreach(var soID in deck)
            {
                if(Managers.Data.CardDictionary.TryGetValue(soID, out var cardSO))
                {
                    resultDeck.Add(new Card(clientID, cardSO));
                }
                else
                {
                    Managers.Logger.Log<GameLogicEx>($"{soID} CardSO not found", colorName: ColorCodes.Server);
                }
            }

            player.Deck = resultDeck;

            OnDataUpdated?.Invoke();
        }

        public void StartGame()
        {
            // 초상화 설정
            // 그냥 랜덤하게 하나씩 쥐어줘 ㅠㅠ 값만 넣어주면 Client쪽에서 알아서 처리해줄텐디 ㅠㅠ
            Random random = new Random();
            int number1 = random.Next(3, 13); // Next의 두 번째 인자는 상한을 포함하지 않으므로 13을 사용
            // 두 번째 숫자 뽑기 (첫 번째 숫자와 중복되지 않도록)
            int number2;
            do
            {
                number2 = random.Next(3, 13);
            } while (number2 == number1);

            Data.Players[0].Portrait = number1;
            Data.Players[1].Portrait = number2;

            // 체력 30
            // 마나 0
            // 덱 섞기
            // 카드 3장 뽑기 
            foreach (var player in Data.Players)
            {
                player.Card.SetHP(30);
                player.SetMaxMana(0);
                ShuffleDeck(player);
                DrawCard(player, out var mulliganTarget, 3);
            }

            // 선 플레이어 설정
            ulong number3 = (ulong)random.Next(0, 2);
            Data.CurrentPlayer = Data.GetPlayer(number3);

            // 후 플레이어는 카드 한 장 드로우
            DrawCard(Data.GetOpponentPlayer(Data.CurrentPlayer), out var _);

            // Send Data Update
            OnDataUpdated?.Invoke();
            // Send Start Game
            OnGameStarted?.Invoke();
        }

        private void ShuffleDeck(Player player)
        {
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

        private void DrawCard(Player player, out List<Card> drawnCards, int count = 1)
        {
            Managers.Logger.Log<GameLogic>("Card draw", colorName: ColorCodes.Server);

            List<Card> outCards = new();

            Card card = null;

            for(int i = 0; i < count; i++)
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
                    Managers.Logger.LogWarning<GameLogic>("Can't draw", colorName: ColorCodes.Server);
                }
            }

            drawnCards = outCards;
        }
    }
}