using System.Collections.Generic;
using UnityEngine.Events;

namespace Marsion.Logic
{
    public interface IGameLogic
    {
        #region Game Logic Operations

        void SetPortrait(Player player, int index);

        void SetHP(Player player, int amount);

        void SetDeck(Player player, List<Card> deck);

        void ShuffleDeck(Player player);

        Card DrawCard(Player player);

        void Damage(IDamageable attacker, IDamageable defender);

        bool CheckDeadCard(Player[] players);

        int GetAlivePlayer();

        #endregion
    }
}