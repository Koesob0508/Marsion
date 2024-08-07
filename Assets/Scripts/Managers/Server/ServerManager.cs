using Marsion.Server;
using Marsion;
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

        private GameData gameData;
        private GameLogic Logic;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            if(IsHost)
            {
                Managers.Logger.Log<ServerManager>("Init Server");

                int playerCount = 2;
                gameData = new GameData(playerCount);
                Logic = new GameLogic(gameData);
                Logic.OnGameStart -= OnGameStart;
                Logic.OnGameStart += OnGameStart;

                Logic.OnUpdate -= OnUpdateData;
                Logic.OnUpdate += OnUpdateData;
            }
        }

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

        [ServerRpc]
        public void ClearServerRpc()
        {
            Logic.OnGameStart -= OnGameStart;
            Logic.OnUpdate -= OnUpdateData;
        }

        /// <summary>
        ///     Server에서 발생하는 이벤트 처리
        /// </summary>
        /// <param name="clientId"></param>
        #region Server Flow

        private void OnClientConnected(ulong clientId)
        {
            if (!IsHost)
            {
                Clear();
                gameObject.SetActive(false);
                return;
            }

            CheckConnectionRpc();
        }

        #endregion

        /// <summary>
        ///     Logic에서 발생한 이벤트를 Client로 전달해주는 역할하는 메서드들 (중복 구현)
        /// </summary>
        #region Logic Flow

        private void OnUpdateData()
        {
            gameData = Logic.GetGameData();
            NetworkGameData networkData = new NetworkGameData();
            networkData.gameData = gameData;
            Managers.Client.UpdateDataClientRpc(networkData);
        }

        private void OnGameStart()
        {
            Managers.Client.GameStartRpc();
        }

        #endregion

        /// <summary>
        ///     Server Manager가 처리해야할 작업
        /// </summary>
        #region Operations

        [Rpc(SendTo.Server)]
        private void CheckConnectionRpc()
        {
            if (AreAllPlayersConnected())
            {
                Managers.Logger.Log<ServerManager>($"Ready to start");

                SetPlayerDeck(gameData.Players[0], p1Deck);
                SetPlayerDeck(gameData.Players[1], p2Deck);

                // 게임 시작
                Logic.StartGame();
            }
        }

        [Rpc(SendTo.Server)]
        public void PlayCardServerRpc(ulong clientID, int index)
        {
            Card card = gameData.Players[clientID].Hand[index];
            Logic.PlayCard(clientID, card);
        }


        private bool AreAllPlayersConnected()
        {
            return Managers.Network.ConnectedClientsList.Count == 2;
        }

        private void SetPlayerDeck(Player player, DeckSO deck)
        {
            player.Deck.Clear();

            foreach (CardSO cardSO in deck.cards)
            {
                if (cardSO != null)
                {
                    Card card = new Card(cardSO);
                    player.Deck.Add(card);
                }
            }
        }

        #endregion
    }
}