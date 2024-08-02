using Unity.Netcode;
using UnityEngine;

namespace Marsion.Client
{
    public class ClientManager : NetworkBehaviour
    {
        public InputManager Input { get; private set; }

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
        }
    }
}