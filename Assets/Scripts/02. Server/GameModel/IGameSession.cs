using System.Collections.Generic;

namespace Marsion
{
    public interface IGameSession
    {
        void Init(IGameModelFactory gameModelFactory);
        void Ready(ulong clientID, List<string> deck);
    }
}