using NUnit.Framework.Constraints;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion.Logic
{
    public class GameLogic : IGameLogic
    {
        GameData _gameData;

        #region Logic Operations

        public GameLogic(GameData gameData)
        {
            _gameData = gameData;
        }

        public GameLogic(IServerGameData gameData)
        {
            
        }

        private void UpdateData()
        {
            Managers.Logger.Log<GameLogic>("Data update", colorName: "yellow");

            OnDataUpdated?.Invoke();
        }

        #endregion

        private List<Player> AlivePlayers = new List<Player>();

        #region Interface Operations

        public event UnityAction OnDataUpdated;

        public event UnityAction OnGameStarted;
        public event UnityAction OnTurnStarted;
        public event UnityAction OnTurnEnded;
        public event UnityAction<int> OnGameEnded;

        public event UnityAction<Player, Card> OnCardDrawn;
        public event UnityAction OnManaChanged;
        public event UnityAction<bool, Player, Card> OnCardPlayed;
        public event UnityAction<bool, Player, Card, int> OnCardSpawned;
        public event UnityAction<Card, Card> OnStartAttack;
        public event UnityAction OnCardBeforeDead;
        public event UnityAction OnCardAfterDead;

        public GameData GetGameData()
        {
            return _gameData;
        }

        // Flow
        public void StartGame()
        {
            foreach (var player in _gameData.Players)
            {
                player.Card.SetHP(30);
                ShuffleDeck(player.Deck);
                for(int i = 0; i < 3; i++)
                    DrawCard(player);
            }

            _gameData.CurrentPlayer = _gameData.GetPlayer(0);

            Managers.Logger.Log<GameLogic>("Game start", colorName : "yellow");
            UpdateData();
            OnGameStarted?.Invoke();

            StartTurn();
        }

        public void EndGame()
        {
            Managers.Logger.Log<GameLogic>("Game end", colorName: "yellow");

            int winner = -1;

            foreach(var player in AlivePlayers)
            {
                winner = (int)player.ClientID;
            }

            OnGameEnded?.Invoke(winner);
        }

        public void StartTurn()
        {
            Managers.Logger.Log<GameLogic>("Turn start", colorName: "yellow");

            _gameData.TurnCount++;

            if (_gameData.TurnCount != 1)
                DrawCard(GetGameData().CurrentPlayer);

            if (GetGameData().CurrentPlayer.MaxMana < 10)
                GetGameData().CurrentPlayer.IncreaseMaxMana(1);

            GetGameData().CurrentPlayer.RestoreAllMana();
            UpdateData();
            OnManaChanged?.Invoke();
            OnTurnStarted?.Invoke();
        }

        public void EndTurn()
        {
            Managers.Logger.Log<GameLogic>("Turn end", colorName: "yellow");

            _gameData.CurrentPlayer = _gameData.CurrentPlayer == _gameData.GetPlayer(0) ? _gameData.GetPlayer(1) : _gameData.GetPlayer(0);

            UpdateData();
            OnTurnEnded?.Invoke();

            StartTurn();
        }

        // Game Operations

        public void ShuffleDeck(List<Card> deck)
        {
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

        public void DrawCard(Player player)
        {
            Managers.Logger.Log<GameLogic>("Card draw", colorName: "yellow");

            Card card = null;

            if (player.Deck.Count > 0 && player.Hand.Count < 10)
            {
                card = player.Deck[0];
                player.Deck.RemoveAt(0);
                player.Hand.Add(card);

                UpdateData();
                OnCardDrawn?.Invoke(player, card);
            }
            else
            {
                Managers.Logger.Log<GameLogic>("Can't draw", colorName: "yellow");
            }
        }

        public void PlayCard(Player player, Card card)
        {
            // Player의 턴이 아니라면 false
            // Field에 넣을 공간이 없다면 false
            // 현재 Player의 마나가 Card의 마나보다 적다면 false
            if (!(player.Mana >= card.Mana)) return;

            Managers.Logger.Log<GameLogic>("Card play", colorName: "yellow");

            player.Hand.Remove(card);

            UpdateData();
            OnManaChanged?.Invoke();
            OnCardPlayed?.Invoke(true, player, card);
        }

        public void SpanwCard(Player player, Card card, int index)
        {
            Managers.Logger.Log<GameLogic>("Card spawn", colorName: "yellow");

            player.Field.Insert(index, card);

            UpdateData();
            OnCardSpawned?.Invoke(true, player, card, index);
        }

        public void TryPlayAndSpawnCard(Player player, Card card, int index)
        {
            // Player의 턴이 아니라면 false
            // Field에 넣을 공간이 없다면 false
            // 현재 Player의 마나가 Card의 마나보다 적다면 false
            if (!(player.Mana >= card.Mana))
            {
                Managers.Logger.Log<GameLogic>("그럴 수 없어요.", colorName: "yellow");
                OnCardPlayed?.Invoke(false, player, card);
                OnCardSpawned?.Invoke(false, player, card, index);
                return;
            }

            player.PayMana(card.Mana);
            player.Hand.Remove(card);
            player.Field.Insert(index, card);

            UpdateData();
            OnManaChanged?.Invoke();
            OnCardPlayed?.Invoke(true, player, card);
            OnCardSpawned?.Invoke(true, player, card, index);
        }

        public void TryAttack(Player attackPlayer, Card attacker, Player defenderPlayer, Card defender)
        {
            Managers.Logger.Log<GameLogic>("Card attack", colorName: "yellow");
            // 각 Player Field 확인해서 조건 검색

            Damage(attacker, defender);
            OnDataUpdated?.Invoke();
            OnStartAttack?.Invoke(attacker, defender);

            CheckDeadCard();
            OnDataUpdated?.Invoke();
            OnCardBeforeDead?.Invoke();
            OnCardAfterDead?.Invoke();
        }

        private void Damage(IDamageable attacker, IDamageable defender)
        {
            attacker.Damage(defender.Attack);
            defender.Damage(attacker.Attack);
        }

        private void CheckDeadCard()
        {
            foreach(Player player in _gameData.Players)
            {
                if(player.Card.HP <= 0)
                {
                    player.Card.Die();
                }

                foreach(Card card in player.Field)
                {
                    if (card.HP <= 0)
                    {
                        card.Die();
                    }
                }
            }

            AlivePlayers.Clear();

            foreach(Player player in _gameData.Players)
            {
                if (!player.Card.IsDead)
                    AlivePlayers.Add(player);
            }

            if (AlivePlayers.Count != 2)
                EndGame();
        }

        private void RemoveDeadCard()
        {
            foreach (Player player in _gameData.Players)
            {
                for (int i = player.Field.Count - 1; i >= 0; i--)
                {
                    if (player.Field[i].IsDead)
                    {
                        player.Field.RemoveAt(i);
                    }
                }
            }
        }

        #endregion
    }
}