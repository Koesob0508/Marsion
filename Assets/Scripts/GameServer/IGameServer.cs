using System.Collections.Generic;

namespace Marsion
{
    public interface IGameServer
    {
        void Init();
        void AddPlayer(ClientData clientData);
    }
}