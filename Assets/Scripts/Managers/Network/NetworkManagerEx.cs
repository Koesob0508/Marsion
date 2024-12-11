using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections;

namespace Marsion
{
    public class NetworkManagerEx : INetworkManagerEx
    {
        private readonly INetworkWrapper networkWrapper;
        private readonly NetworkEventHandler eventHandler;

        public NetworkManagerEx(INetworkWrapper networkWrapper)
        {
            this.networkWrapper = networkWrapper ?? throw new ArgumentNullException(nameof(networkWrapper));
            this.eventHandler = new NetworkEventHandler(networkWrapper);
        }

        // Properties
        public ulong LocalClientID => networkWrapper.LocalClientID;
        public ulong ServerClientID => networkWrapper.ServerClientID;
        public bool IsServer => networkWrapper.IsServer;
        public bool IsClient => networkWrapper.IsClient;
        public bool IsHost => IsClient && IsServer;
        public bool IsConnected => IsClient || IsServer;
        public IReadOnlyList<ulong> ConnectedClientsIDs => networkWrapper.ConnectedClientsIDs;

        // Events
        public event Action OnConnect
        {
            add => eventHandler.OnConnect += value;
            remove => eventHandler.OnConnect -= value;
        }
        public event Action OnDisconnect
        {
            add => eventHandler.OnDisconnect += value;
            remove => eventHandler.OnDisconnect -= value;
        }
        public event Action<ulong> OnClientConnected
        {
            add => eventHandler.OnClientConnected += value;
            remove => eventHandler.OnClientConnected -= value;
        }
        public event Action<ulong> OnClientDisconnected
        {
            add => eventHandler.OnClientDisconnected += value;
            remove => eventHandler.OnClientDisconnected -= value;
        }

        // Messaging
        protected readonly Dictionary<string, Action<ulong, FastBufferReader>> messageHandlers = new();

        public void SubscribeMessage(string messageType, Action<ulong, FastBufferReader> handler)
        {
            if (messageHandlers.ContainsKey(messageType))
            {
                throw new InvalidOperationException($"A message handler for '{messageType}' is already registered.");
            }

            messageHandlers[messageType] = handler;
            RegisterMessage(messageType, handler);
        }

        public void UnsubscribeMessage(string messageType)
        {
            if (messageHandlers.Remove(messageType))
            {
                networkWrapper.CustomMessagingManager.UnregisterNamedMessageHandler(messageType);
            }
        }

        public void SendMessage(string messageType, ulong target, Action<FastBufferWriter> writeAction, NetworkDelivery delivery)
        {
            using (var writer = new FastBufferWriter(128, Allocator.Temp, 1024 * 1024))
            {
                writeAction(writer);
                networkWrapper.CustomMessagingManager.SendMessage(messageType, target, writer, delivery);
            }
        }

        public void BroadcastMessage(string messageType, Action<FastBufferWriter> writeAction, NetworkDelivery delivery)
        {
            using (var writer = new FastBufferWriter(128, Allocator.Temp, 1024 * 1024))
            {
                writeAction(writer);
                networkWrapper.CustomMessagingManager.SendMessage(messageType, ConnectedClientsIDs, writer, delivery);
            }
        }

        // Methods
        public void StartHost()
        {
            networkWrapper.StartHost();
        }

        public void StartServer()
        {
            networkWrapper.StartServer();
        }

        public void StartClient()
        {
            networkWrapper.StartClient();
        }

        public void Shutdown()
        {
            networkWrapper.Shutdown();
        }

        public void Clear()
        {
            UnsubscribeAllMessages();

            eventHandler.Clear();
        }

        // Private methods
        private void RegisterMessage(string messageType, Action<ulong, FastBufferReader> handler)
        {
            try
            {
                networkWrapper.CustomMessagingManager.RegisterNamedMessageHandler(messageType, (clientId, reader) =>
                {
                    if (messageHandlers.TryGetValue(messageType, out var callback))
                    {
                        callback(clientId, reader);
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError<NetworkManagerEx>($"Failed to register message handler for {messageType}: {ex.Message}");
            }
        }

        private void UnsubscribeAllMessages()
        {
            foreach (var messageType in messageHandlers.Keys)
            {
                try
                {
                    networkWrapper.CustomMessagingManager.UnregisterNamedMessageHandler(messageType);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning<NetworkManagerEx>($"Failed to unregister message handler for {messageType}: {ex.Message}");

                }
            }

            messageHandlers.Clear();
        }
    }
}
