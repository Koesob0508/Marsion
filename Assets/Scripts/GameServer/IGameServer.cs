using Marsion.Logic;
using Unity.Netcode;

namespace Marsion.Server
{
    public interface IGameServer
    {
        void Init();
        void Clear();
        void DrawCardRpc(ulong clientID);
        void PlayCardRpc(ulong clientID, string cardUID);
        void PlayAndSpawnCardRpc(ulong clientID, string cardUID, int index);

        void DrawButtonRpc(ulong clientID);
    }
}