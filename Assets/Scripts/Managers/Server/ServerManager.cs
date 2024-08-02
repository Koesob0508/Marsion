using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion.Server
{
    public class ServerManager : NetworkBehaviour
    {
        public bool IsConnected { get; private set; }
        public UnityAction OnConnect;
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
            if(Managers.Network.IsHost)
            {
                Managers.Logger.Log<ServerManager>("Host");

                InitServerManagers();

                if (AreAllPlayersConnected())
                    Game.StartServerRpc();
                
            }
            else if (Managers.Network.IsClient)
            {
                if (AreAllPlayersConnected())
                    Managers.Logger.Log<ServerManager>("Client");
            }
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