using System.Collections.Generic;

namespace Marsion
{
    public interface IPlayer
    {
        ulong PlayerID { get; }
        string HeroID { get; }
        int Health { get; }
        int MaxHealth { get; }
        int Mana { get; }
        int MaxMana { get; }
        List<ICard> Deck { get; }
        List<ICard> Hand { get; }
        List<ICard> Field { get; }
        List<ICard> Grave { get; }

        // TODO : 아래 변수는 리팩토링
        ICard PlayerCard { get; }
        string Portrait { get; }

        // TODO : 아래 함수들 리팩토링
        void Init();
        void SetPlayerID(ulong playerID);
        void SetPlayerPortrait(string portraitID);
        void SetDeck(List<ICard> deck);
        bool TryGetPlayerCard(string uid, out ICard card);
        bool TryGetHandCard(string uid, out ICard card);
        bool TryGetFieldCard(string uid, out ICard card);
        void SetMaxHealth(int amount);
        void SetMaxMana(int amount);
        void IncreaseMaxMana(int amount);
        void RestoreMana(int amount);
        void RestoreAllMana();
        void PayMana(int amount);
    }
}