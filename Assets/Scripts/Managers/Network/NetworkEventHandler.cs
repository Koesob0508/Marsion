using System;

namespace Marsion
{
    public class NetworkEventHandler
    {
        private readonly INetworkWrapper networkWrapper;

        public event Action OnConnect;
        public event Action OnDisconnect;
        public event Action<ulong> OnClientConnected;
        public event Action<ulong> OnClientDisconnected;

        public NetworkEventHandler(INetworkWrapper networkWrapper)
        {
            this.networkWrapper = networkWrapper ?? throw new ArgumentNullException(nameof(networkWrapper));

            Init();
        }

        public void Init()
        {
            networkWrapper.OnConnect += HandleConnect;
            networkWrapper.OnDisconnect += HandleDisconnect;
            networkWrapper.OnClientConnectedCallback += HandleClientConnected;
            networkWrapper.OnClientDisconnectCallback += HandleClientDisconnected;
        }

        public void Clear()
        {
            // Unsubscribe all handlers
            networkWrapper.OnConnect -= HandleConnect;
            networkWrapper.OnDisconnect -= HandleDisconnect;
            networkWrapper.OnClientConnectedCallback -= HandleClientConnected;
            networkWrapper.OnClientDisconnectCallback -= HandleClientDisconnected;

            // Clear all local event handlers
            OnConnect = null;
            OnDisconnect = null;
            OnClientConnected = null;
            OnClientDisconnected = null;
        }

        private void HandleConnect()
        {
            if (OnConnect == null) return;

            foreach (var handler in OnConnect.GetInvocationList())
            {
                try
                {
                    ((Action)handler)?.Invoke();
                }
                catch (Exception ex)
                {
                    Logger.LogError<NetworkEventHandler>($"Exception in OnConnect handler: {ex.Message}");
                }
            }
        }

        private void HandleDisconnect()
        {
            OnDisconnect?.Invoke();
        }

        private void HandleClientConnected(ulong clientId)
        {
            OnClientConnected?.Invoke(clientId);
        }

        private void HandleClientDisconnected(ulong clientId)
        {
            OnClientDisconnected?.Invoke(clientId);
        }
    }
}