using System;
using System.Collections.Generic;
using Unity.Netcode;

namespace Marsion
{
    public interface INetworkManagerEx
    {
        ulong LocalClientID { get; }
        ulong ServerClientID { get; }
        bool IsServer { get; }
        bool IsClient { get; }
        bool IsHost { get; }
        bool IsConnected { get; }
        IReadOnlyList<ulong> ConnectedClientsIDs { get; }

        event Action OnConnect;
        event Action OnDisconnect;
        event Action<ulong> OnClientConnected;
        event Action<ulong> OnClientDisconnected;

        void SubscribeMessage(string messageType, Action<ulong, FastBufferReader> handler);
        void UnsubscribeMessage(string messageType);
        void SendMessage(string messageType, ulong target, Action<FastBufferWriter> writeAction, NetworkDelivery deliery);
        void BroadcastMessage(string messageType, Action<FastBufferWriter> writeAction, NetworkDelivery delivery);
        void StartHost();
        void StartServer();
        void StartClient();
        void Shutdown();
        void Clear();
    }
}