using Marsion.CardView;
using Marsion.Logic;
using Marsion.Tool;
using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface IGameClient
    {
        IGameData Data { get; }
        ulong PlayerID { get; }
        ulong EnemyID { get; }
        IFieldView PlayerField { get; }

        event Action OnDataUpdated;
        event Action OnGameStarted;
        event Action OnGameReset;
        event Action OnTurnStarted;
        event Action OnTurnEnded;
        event Action OnManaChanged;
        event Action<IPlayer, ICard> OnCardDrawn;
        event Action<bool, IPlayer, ICard> OnCardPlayed;
        event Action<bool, IPlayer, ICard, int> OnCardSpawned;
        event Action<Sequencer.Sequence, IPlayer, ICard, IPlayer, ICard> OnAttackStarted;
        event Action<List<string>> OnCardDied;

        void Init();

        void SendTurnEnd();

        void SendTryAttack(ICard attacker, ICard defender);
        void SendTrySpawnCard(ICard card, int index);

        bool IsMine(ulong id);
        bool IsMine(IPlayer player);
        bool IsMine(ICard card);
        bool IsMyTurn();
        ICard GetCard(CardType type, ulong playerID, string cardUID);
        ICharacterView GetCharacter(ulong playerID, string cardUID);
    }
}