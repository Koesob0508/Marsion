using Marsion.Logic;
using System;
using System.Collections.Generic;

namespace Marsion
{
    public class GameLogicEx
    {
        private readonly CommandInvoker commandInvoker = new();

        public GameData Data;
        public event Action OnDataUpdated;
        public event Action OnGameStarted;
        public event Action OnManaChanged;
        public event Action OnTurnStarted;
        public event Action OnTurnEnded;
        public event Action<ulong, string> OnCardDrawn;
        public event Action<bool, ulong, string> OnCardPlayed;
        public event Action<bool, ulong, string, int> OnCardSpawned;
        public event Action<bool, ulong, string, ulong, string> OnCardAttacked;
        public event Action<List<string>> OnCardDied;
        public event Action<ulong> OnGameEnded;

        public GameLogicEx(GameData data)
        {
            Data = data;
        }

        public void SetPlayerDeck(ulong clientID, List<string> deck)
        {
            Logger.Log<GameLogicEx>($"Set player deck", colorName: ColorCodes.Logic);
            Player player = Data.GetPlayer(clientID);
            List<Card> resultDeck = new List<Card>();

            foreach(var soID in deck)
            {
                if(Managers.Instance.Data.GetDictionary<CardSO>().TryGetValue(soID, out var cardSO))
                {
                    resultDeck.Add(new Card(clientID, cardSO));
                }
                else
                {
                    Logger.Log<GameLogicEx>($"{soID} CardSO not found", colorName: ColorCodes.Logic);
                }
            }

            player.Deck = resultDeck;

            OnDataUpdated?.Invoke();
        }

        public void StartGame()
        {
            Logger.Log<GameLogicEx>($"Start Game", colorName: ColorCodes.Logic);

            // 초상화 설정
            Random random = new Random();
            int number1 = random.Next(3, 13); // Next의 두 번째 인자는 상한을 포함하지 않으므로 13을 사용
            // 두 번째 숫자 뽑기 (첫 번째 숫자와 중복되지 않도록)
            int number2;
            do
            {
                number2 = random.Next(3, 13);
            } while (number2 == number1);

            Data.Players[0].Portrait = number1.ToString();
            Data.Players[1].Portrait = number2.ToString();

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

            StartTurn();
        }

        private void StartTurn()
        {
            Logger.Log<GameLogicEx>($"Start Turn", colorName: ColorCodes.Logic);

            Data.TurnCount++;

            if (Data.CurrentPlayer.MaxMana < 10)
                Data.CurrentPlayer.IncreaseMaxMana(1);

            Data.CurrentPlayer.RestoreAllMana();

            DrawCard(Data.CurrentPlayer, out var card);

            OnDataUpdated?.Invoke();
            OnManaChanged?.Invoke();
            OnTurnStarted?.Invoke();
            OnCardDrawn?.Invoke(Data.CurrentPlayer.PlayerID, card.UID);
        }

        public void EndTurn()
        {
            Logger.Log<GameLogicEx>($"End turn", colorName: ColorCodes.Logic);

            Data.CurrentPlayer = Data.CurrentPlayer == Data.GetPlayer(0) ? Data.GetPlayer(1) : Data.GetPlayer(0);

            OnDataUpdated?.Invoke();
            OnTurnEnded?.Invoke();

            StartTurn();
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

        public void DrawCard(Player player, out Card drawnCard)
        {
            Logger.Log<GameLogic>("Draw a card", colorName: ColorCodes.Logic);

            Card card = null;

            if (player.Deck.Count > 0 && player.Hand.Count < 10)
            {
                card = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(card);
            }
            else
            {
                Logger.LogWarning<GameLogic>("Can't draw", colorName: ColorCodes.Logic);
            }

            drawnCard = card;
        }

        public void DrawCard(Player player, out List<Card> drawnCards, int count = 1)
        {
            Logger.Log<GameLogic>("Draw cards", colorName: ColorCodes.Logic);

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
                    Logger.LogWarning<GameLogic>("Can't draw", colorName: ColorCodes.Logic);
                }
            }

            drawnCards = outCards;
        }

        public void TrySpawnCard(Player player, Card card, int index)
        {
            Logger.Log<GameLogic>("Try spawn card", colorName: ColorCodes.Logic);

            if (!(player.Mana >= card.Mana))
            {
                Logger.Log<GameLogicEx>("Spawn try failed", colorName: ColorCodes.Logic);
                OnCardPlayed?.Invoke(false, player.PlayerID, card.UID);
                OnCardSpawned?.Invoke(false, player.PlayerID, card.UID, index);

                return;
            }

            var playCardCommand = new PlayCardCommand(player, card, index);
            commandInvoker.AddCommand(playCardCommand);
            commandInvoker.ExecuteCommands();

            OnCardPlayed?.Invoke(true, player.PlayerID, card.UID);
            OnCardSpawned?.Invoke(true, player.PlayerID, card.UID, index);
            OnDataUpdated?.Invoke();
            OnManaChanged?.Invoke();
        }

        public void TryAttack(Player attackPlayer, Card attacker, Player defendPlayer, Card defender)
        {
            var attackCommand = new AttackCommand(attackPlayer, attacker, defendPlayer, defender);
            commandInvoker.AddCommand(attackCommand);
            commandInvoker.ExecuteCommands();

            OnDataUpdated?.Invoke();
            OnCardAttacked?.Invoke(true, attackPlayer.PlayerID, attacker.UID, defendPlayer.PlayerID, defender.UID);

            List<string> deadCardUIDs = CheckDeadCard();

            OnDataUpdated?.Invoke();
            OnCardDied?.Invoke(deadCardUIDs);

            RemoveDeadCard();

            OnDataUpdated?.Invoke();

            List<ulong> alivePlayerIDs = new();

            foreach(var player in Data.Players)
            {
                if(player.Card.Health > 0)
                {
                    alivePlayerIDs.Add(player.PlayerID);
                }
            }

            if(alivePlayerIDs.Count == 1)
            {
                OnGameEnded?.Invoke(alivePlayerIDs[0]);
            }
            else if(alivePlayerIDs.Count == 0)
            {
                OnGameEnded?.Invoke(1000);
            }
        }

        private List<string> CheckDeadCard()
        {
            List<string> result = new();

            foreach (var player in Data.Players)
            {
                foreach (Card card in player.Field)
                {
                    if(card.Health <= 0)
                    {
                        card.Die();
                        result.Add(card.UID);
                    }
                }
            }

            return result;
        }

        private void RemoveDeadCard()
        {
            List<Card> deadCards = new();

            foreach (var player in Data.Players)
            {
                foreach (Card card in player.Field)
                {
                    if (card.IsDead)
                    {
                        deadCards.Add(card);
                    }
                }
            }

            foreach(var card in deadCards)
            {
                foreach(var player in Data.Players)
                {
                    if(player.Field.Contains(card))
                    {
                        player.Field.Remove(card);
                    }
                }
            }
        }
    }
}