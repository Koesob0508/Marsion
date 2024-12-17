using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

namespace Marsion
{
    public class NetworkManagerEx : INetworkManagerEx
    {
        private readonly INetworkManagerWrapper networkWrapper;
        private readonly ICustomMessagingManager messagingManager;

        public NetworkManagerEx(INetworkManagerWrapper networkWrapper)
        {
            this.networkWrapper = networkWrapper ?? throw new ArgumentNullException(nameof(networkWrapper));
            messagingManager = new CustomMessagingManagerWrapper(this.networkWrapper);
        }

        // Properties
        public ulong LocalID => networkWrapper.LocalClientID;
        public ulong ServerID => networkWrapper.ServerClientID;
        public bool IsServer => networkWrapper.IsServer;
        public bool IsClient => networkWrapper.IsClient;
        public bool IsHost => IsClient && IsServer;
        public bool IsConnected => IsClient || IsServer;
        public IReadOnlyList<ulong> ConnectedClientsIDs => networkWrapper.ConnectedClientsIDs;

        // Events
        public event Action OnConnect
        {
            add => networkWrapper.OnConnect += value;
            remove => networkWrapper.OnConnect -= value;
        }
        public event Action OnDisconnect
        {
            add => networkWrapper.OnDisconnect += value;
            remove => networkWrapper.OnDisconnect -= value;
        }
        public event Action<ulong> OnClientConnected
        {
            add => networkWrapper.OnClientConnected += value;
            remove => networkWrapper.OnClientConnected -= value;
        }
        public event Action<ulong> OnClientDisconnected
        {
            add => networkWrapper.OnClientDisconnected += value;
            remove => networkWrapper.OnClientDisconnected -= value;
        }

        public void SubscribeMessage(string messageType, Action<ulong, FastBufferReader> handler)
        {
            messagingManager.SubscribeMessage(messageType, handler);
        }

        public void UnsubscribeMessage(string messageType, Action<ulong, FastBufferReader> handler)
        {
            messagingManager.UnsubscribeMessage(messageType, handler);
        }

        public void SendMessage(string messageType, ulong target, Action<FastBufferWriter> writeAction, NetworkDelivery delivery)
        {
            using (var writer = new FastBufferWriter(128, Allocator.Temp, 1024 * 1024))
            {
                writeAction(writer);
                networkWrapper.CustomMessagingManager.SendNamedMessage(messageType, target, writer, delivery);
            }
        }

        public void BroadcastMessage(string messageType, Action<FastBufferWriter> writeAction, NetworkDelivery delivery)
        {
            using (var writer = new FastBufferWriter(128, Allocator.Temp, 1024 * 1024))
            {
                writeAction(writer);
                networkWrapper.CustomMessagingManager.SendNamedMessage(messageType, ConnectedClientsIDs, writer, delivery);
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
            messagingManager.UnsubscribeAllMessages();
        }
    }
}
