using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public class NetworkManagerWrapper : INetworkManagerWrapper
    {
        private readonly NetworkManager networkManager;
        public IReadOnlyList<ulong> ConnectedClientsIDs => networkManager.ConnectedClientsIds;
        public CustomMessagingManager CustomMessagingManager => networkManager.CustomMessagingManager;

        public NetworkManagerWrapper(NetworkManager networkManager)
        {
            this.networkManager = networkManager ?? throw new ArgumentNullException(nameof(networkManager));
            this.networkManager.OnClientConnectedCallback += OnClientConnectedHandler;
            this.networkManager.OnClientDisconnectCallback += OnClientDisconnectedHandler;
        }

        public void Clear()
        {
            networkManager.OnClientConnectedCallback -= OnClientConnectedHandler;
            networkManager.OnClientDisconnectCallback -= OnClientDisconnectedHandler;
        }

        public ulong LocalClientID => networkManager.LocalClientId;
        public ulong ServerClientID => NetworkManager.ServerClientId;
        public bool IsServer => networkManager.IsServer;
        public bool IsClient => networkManager.IsClient;
        public bool IsConnected => IsServer || IsClient;

        public event Action OnConnect;
        public event Action OnDisconnect;
        public event Action<ulong> OnClientConnected;
        public event Action<ulong> OnClientDisconnected;

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

        private void OnClientConnectedHandler(ulong clientID)
        {
            OnClientConnected?.Invoke(clientID);
        }

        private void OnClientDisconnectedHandler(ulong clientID)
        {
            OnClientDisconnected?.Invoke(clientID);
        }
    }
}