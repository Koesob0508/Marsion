using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion.Server
{
    public class ServerManager : NetworkBehaviour
    {
        public bool IsConnected { get; private set; }

        public DeckSO p1Deck;
        public DeckSO p2Deck;


        #region UnityActions

        public UnityAction OnHostConnected;

        #endregion

        private GameFlow Flow;
        public GameManager Game { get; private set; }

        public void Init()
        {
            if (Managers.Network != null)
            {
                Managers.Network.OnClientConnectedCallback -= OnClientConnected;
                Managers.Network.OnClientConnectedCallback += OnClientConnected;
            }
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

        public void ConnectHost()
        {
            InitServerManager();

            OnHostConnected?.Invoke();
        }

        [ServerRpc]
        private void CheckConnectionServerRpc()
        {
            if (AreAllPlayersConnected())
            {
                Managers.Logger.Log<ServerManager>($"Ready to start");

                Flow.SetFirstPlayerDeck(p1Deck);
                Flow.SetSecondPlayerDeck(p2Deck);
                Flow.SetCurrentPlayer();
                Flow.InitialDeckShuffle();
                Flow.StartGame();
            }
        }

        private void OnGameStart()
        {
            Managers.Client.GameStartClientRpc();
        }

        private void InitServerManager()
        {
            Flow = new GameFlow();
            Flow.onGameStart += OnGameStart;

            //Game = new GameManager();
            //Game.Init();
        }

        private bool AreAllPlayersConnected()
        {
            return Managers.Network.ConnectedClientsList.Count == 2;
        }
    }
}