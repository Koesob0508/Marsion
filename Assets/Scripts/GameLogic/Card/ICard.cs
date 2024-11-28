using System;

namespace Marsion
{
    public interface ICard
    {
        event Action OnPlay;
        event Action OnLastWill;
    }
}