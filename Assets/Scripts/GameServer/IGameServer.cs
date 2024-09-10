using Marsion.Logic;
using System;
using Unity.Netcode;

namespace Marsion.Server
{
    public interface IGameServer
    {
        // Event
        event Action OnGameStarted;

        // Managers
        void Init();
        void Clear();

        // Rpc
        void DrawCardRpc(ulong clientID);
        void PlayCardRpc(ulong clientID, string cardUID);
        void TryPlayAndSpawnCardRpc(ulong clientID, string cardUID, int index);
        void TurnEndRpc();
        void TryAttackRpc(ulong attackerID, string attackerUID, ulong defenderID, string defenderUID);
    }
}