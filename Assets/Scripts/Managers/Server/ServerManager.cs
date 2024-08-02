using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion.Server
{
    public class ServerManager : NetworkBehaviour
    {
        public bool IsConnected { get; private set; }
        public UnityAction OnConnect;

        private GameFlow Flow;
        public GameManager Game { get; private set; }

        public void Init()
        {
            Debug.Log("InitServerRpc");

            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= OnClientConnected;
                Managers.Network.OnClientConnectedCallback += OnClientConnected;
            }

            Flow = new GameFlow();

            Flow.onGameStart += OnGameStart;
        }

        public void Clear()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= OnClientConnected;
            }
        }

        private void OnClientConnected(ulong clinetId)
        {
            if (Managers.Network.IsHost)
            {
                CheckConnectionServerRpc();
            }
        }

        [ServerRpc]
        private void CheckConnectionServerRpc()
        {
            if (AreAllPlayersConnected())
            {
                Managers.Logger.Log<ServerManager>($"Ready to start");
                Flow.StartGame();
            }
        }

        private void OnGameStart()
        {
            Debug.Log("OnGameStart");
            Managers.Client.GameStartClientRpc();
        }

        private void InitServerManagers()
        {
            Game = new GameManager();
            Game.Init();
        }

        private bool AreAllPlayersConnected()
        {
            return Managers.Network.ConnectedClientsList.Count == 2;
        }
    }
}