using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public interface INetworkManagerWrapper
    {
        ulong LocalClientID { get; }
        ulong ServerClientID { get; }
        bool IsServer { get; }
        bool IsClient { get; }
        bool IsConnected { get; }
        void StartHost();
        void StartServer();
        void StartClient();
        void Shutdown();
        IReadOnlyList<ulong> ConnectedClientsIDs { get; }
        CustomMessagingManager CustomMessagingManager { get; }
        event Action OnConnect;
        event Action OnDisconnect;
        event Action<ulong> OnClientConnected;
        event Action<ulong> OnClientDisconnected;
    }
}