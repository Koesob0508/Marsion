using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion.Logic
{
    public interface IGameLogic
    {
        event UnityAction<Card, Card> OnStartAttack;
        event UnityAction OnCardBeforeDead;
        event UnityAction OnCardAfterDead;

        #region Game Logic Operations

        void SetPortrait(Player player, int index);

        void SetHP(Player player, int amount);

        void SetDeck(Player player, DeckSO deck);

        void ShuffleDeck(Player player);

        Card DrawCard(Player player);

        void Damage(IDamageable attacker, IDamageable defender);

        bool CheckDeadCard(Player[] players);

        int GetAlivePlayer();

        #endregion
    }
}