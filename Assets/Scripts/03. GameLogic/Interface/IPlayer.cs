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
    }
}