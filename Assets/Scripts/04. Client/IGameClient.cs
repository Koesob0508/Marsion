using Marsion.CardView;
using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface IGameClient
    {
        DefaultGameData Data { get; }
        ulong PlayerID { get; }
        ulong EnemyID { get; }
        IFieldView PlayerField { get; }

        event Action OnDataUpdated;
        event Action OnGameStarted;
        event Action OnGameReset;
        event Action OnTurnStarted;
        event Action OnTurnEnded;
        event Action OnManaChanged;
        event Action<DefaultPlayer, ICard> OnCardDrawn;
        event Action<bool, DefaultPlayer, Card> OnCardPlayed;
        event Action<bool, DefaultPlayer, Card, int> OnCardSpawned;
        event Action<Sequencer.Sequence, DefaultPlayer, Card, DefaultPlayer, Card> OnAttackStarted;
        event Action<List<string>> OnCardDied;

        void Init();

        void SendTurnEnd();

        void SendTryAttack(Card attacker, Card defender);
        void SendTrySpawnCard(ICard card, int index);

        bool IsMine(ulong id);
        bool IsMine(DefaultPlayer player);
        bool IsMine(Card card);
        bool IsMyTurn();
        Card GetCard(CardType type, ulong playerID, string cardUID);
        ICharacterView GetCharacter(ulong playerID, string cardUID);
    }
}