using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public class NetworkManagerWrapper : INetworkWrapper
    {
        private readonly NetworkManager networkManager;

        public NetworkManagerWrapper(NetworkManager networkManager)
        {
            this.networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
        }

        public ulong LocalClientID => networkManager.LocalClientId;
        public ulong ServerClientID => NetworkManager.ServerClientId;
        public bool IsServer => networkManager.IsServer;
        public bool IsClient => networkManager.IsClient;

        public IReadOnlyList<ulong> ConnectedClientsIDs =>
            networkManager.ConnectedClientsIds;
        public ICustomMessagingManager CustomMessagingManager =>
            new CustomMessagingManagerWrapper(networkManager.CustomMessagingManager);

        public event Action OnConnect;
        public event Action OnDisconnect;
        public event Action<ulong> OnClientConnectedCallback;
        public event Action<ulong> OnClientDisconnectCallback;


        public void StartHost()
        {
            networkManager.StartHost();
            OnConnect?.Invoke();
        }

        public void StartServer()
        {
            networkManager.StartServer();
            OnConnect?.Invoke();
        }

        public void StartClient()
        {
            networkManager.StartClient();
            OnConnect?.Invoke();
        }

        public void Shutdown()
        {
            networkManager.Shutdown();
            OnDisconnect?.Invoke();
        }
    }
}