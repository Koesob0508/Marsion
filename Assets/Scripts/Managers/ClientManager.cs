using Unity.Netcode;

namespace Marsion.Client
{
    public class ClientManager
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