using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Marsion
{
    [RequireComponent(typeof(NetworkManager))]
    [RequireComponent(typeof(MarsTransport))]
    public class MarsNetwork : MonoBehaviour
    {
        private NetworkManager network;
        private MarsTransport transport;
        private NetworkMessaging messaging;

        private bool IsConnected = false;

        public NetworkManager NetworkManager => network;

        private const int messageSize = 1024 * 1024;
        public static int MessageSizeMax { get { return messageSize; } }

        // ID of this client (if host, will be same than ServerID), changes for every reconnection, assigned by Netcode
        public ulong ClientID { get { return network.LocalClientId; } }
        public ulong ServerID { get { return NetworkManager.ServerClientId; } }
        public bool IsServer { get { return network.IsServer; } }
        public bool IsClient { get { return network.IsClient; } }
        public bool IsHost { get { return IsClient && IsServer; } }
        public bool IsOnline { get { return IsActive(); } }
        public IReadOnlyList<ulong> ConnectedClientIDs { get { return network.ConnectedClientsIds; } }
        public int ConnectedClientsCount { get { return network.ConnectedClientsList.Count; } }

        public MarsNetwork Get()
        {
            return this;
        }

        public bool IsActive()
        {
            return network.IsServer || network.IsClient;
        }

        public IReadOnlyList<ulong> GetClientIDs()
        {
            return network.ConnectedClientsIds;
        }

        public NetworkMessaging Messaging { get { return messaging; } }

        // Server & Client events
        public event Action OnConnect;    // 네트워크 접속 그 자체를 의미하는 이벤트
        public event Action OnDisconnect; // 네트워크 연결 끊김에 대한 이벤트

        // Server only events
        public event Action<ulong> OnClientJoin;  // Host, Server 기준에서 새로운 Client 연결 이벤트
        public event Action<ulong> OnClientQuit;  // Host, Server 기준에서 Client 종료 이벤트
        // public event Action<ulong> OnClientReady; //Server event when any client become ready

        public void Init()
        {
            Managers.Logger.Log<MarsNetwork>("Network initialized", colorName: ColorCodes.Server);

            network = GetComponent<NetworkManager>();
            transport = GetComponent<MarsTransport>();
            messaging = new NetworkMessaging(this);

            //network.ConnectionApprovalCallback += CheckApproval;
            network.OnClientConnectedCallback += OnClientConnected;
            network.OnClientDisconnectCallback += OnClientDisconnected;
        }

        public void StartHost()
        {
            Managers.Logger.Log<MarsNetwork>($"Start host", colorName: "#00FFFF");
            network.StartHost();
            AfterConnect();
        }

        public void StartServer()
        {
            Managers.Logger.Log<MarsNetwork>($"Start server", colorName: "#00FFFF");
            network.StartServer();
            AfterConnect();
        }

        public void StartClient()
        {
            Managers.Logger.Log<MarsNetwork>($"Start client", colorName: "#00FFFF");
            network.StartClient();
        }

        public void Disconnect()
        {
            if (!IsClient && !IsServer) return;

            Managers.Logger.Log<MarsNetwork>($"Disconnect", colorName: "#00FFFF");
            network.Shutdown();
            AfterDisconnect();
        }

        //private void CheckApproval(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        //{
        //    bool approved = true;
        //    response.Approved = approved;
        //}

        private void OnClientConnected(ulong clientID)
        {
            if (IsServer && clientID != ServerID)
            {
                Managers.Logger.Log<MarsNetwork>($"Client : {clientID} connected", colorName: "#00FFFF");
                OnClientJoin?.Invoke(clientID);
            }

            if (!IsServer)
                AfterConnect();
        }

        private void OnClientDisconnected(ulong clientID)
        {
            if (IsServer && clientID != ServerID)
            {
                Managers.Logger.Log<MarsNetwork>($"Client : {clientID} disconnected", colorName: "#00FFFF");
                OnClientQuit?.Invoke(clientID);
            }

            if (ClientID == clientID || clientID == ServerID)
                AfterDisconnect();
        }

        private void AfterConnect()
        {
            if (IsConnected) return;

            IsConnected = true;
            OnConnect?.Invoke();
        }

        private void AfterDisconnect()
        {
            if (!IsConnected) return;

            IsConnected = false;
            OnDisconnect?.Invoke();
        }
    }
}