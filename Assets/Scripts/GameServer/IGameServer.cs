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
        void TurnEndRpc();
        void TryAttackRpc(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);

        void DrawButtonRpc(ulong clientID);
    }
}