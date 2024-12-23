using System.Collections.Generic;

namespace Marsion
{
    public interface IGameSession
    {
        void Init(IGameSessionFactory gameModelFactory);
        void Ready(ulong clientID, List<string> deck);
    }
}