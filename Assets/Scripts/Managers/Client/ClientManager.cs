using Marsion.Clinet;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace Marsion.Client
{
    public class ClientManager : NetworkBehaviour, IClientLogic
    {
        public InputManager Input { get; private set; }
        private UnityAction onStartGame;
        public UnityAction OnStartGame
        {
            get
            {
                return onStartGame;
            }
            set
            {
                onStartGame = value;
            }
        }

        public void Init()
        {
            Input = new InputManager();
        }

        public void Update()
        {
            Input.Update();
        }

        public void Clear()
        {

        }

        [ClientRpc]
        public void GameStartClientRpc()
        {
            Managers.Logger.Log<ClientManager>("Game Start");

            OnStartGame?.Invoke();
        }
    }
}