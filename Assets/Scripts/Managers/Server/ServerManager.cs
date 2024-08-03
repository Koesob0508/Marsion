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

        private GameData GameData;
        private GameLogic Logic;

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
        private void InitServerManager()
        {
            int playerCount = 2;
            GameData = new GameData(playerCount);
            Logic = new GameLogic(GameData);
            Logic.onGameStart += OnGameStart;
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

                Logic.SetBothPlayerDeck(p1Deck, p2Deck);
                Logic.SetCurrentPlayer();
                Logic.InitialDeckShuffle();
                Logic.StartGame();
            }
        }

        private void OnGameStart()
        {
            Managers.Client.GameStartClientRpc();
        }

        

        private bool AreAllPlayersConnected()
        {
            return Managers.Network.ConnectedClientsList.Count == 2;
        }
    }
}