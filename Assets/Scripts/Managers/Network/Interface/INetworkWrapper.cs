using System;
using System.Collections.Generic;

namespace Marsion
{
    public interface INetworkWrapper
    {
        ulong LocalClientID { get; }
        ulong ServerClientID { get; }
        bool IsServer { get; }
        bool IsClient { get; }
        void StartHost();
        void StartServer();
        void StartClient();
        void Shutdown();
        IReadOnlyList<ulong> ConnectedClientsIDs { get; }
        ICustomMessagingManager CustomMessagingManager { get; }
        event Action OnConnect;
        event Action OnDisconnect;
        event Action<ulong> OnClientConnectedCallback;
        event Action<ulong> OnClientDisconnectCallback;
    }
}