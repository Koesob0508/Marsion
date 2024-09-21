﻿using Marsion.Logic;
using System;
using Unity.Netcode;

namespace Marsion.Server
{
    public interface IGameServer
    {
        // Event
        event Action<NetworkGameData> OnDataUpdated;

        event Action OnGameStarted;
        event Action<int> OnGameEnded;
        event Action OnTurnStarted;
        event Action OnTurnEnded;
        
        event Action<ulong, string> OnCardDrawn;
        event Action OnManaChanged;
        event Action<bool, ulong, string> OnCardPlayed;
        event Action<bool, ulong, string, int> OnCardSpawned;
        event Action<ulong, string, ulong, string> OnStartAttack;
        event Action OnDeadCard;

        // Managers
        void Init();
        void Clear();

        // Rpc
        void TurnEndRpc();
        void TryPlayAndSpawnCardRpc(ulong id, string cardUID, int index);
        void TryAttackRpc(ulong attackPlayer, string attackerUID, ulong defendPlayer, string defenderUID);
    }
}