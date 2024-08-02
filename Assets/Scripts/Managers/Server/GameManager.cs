using Unity.Netcode;

namespace Marsion.Server
{
    public class GameManager
    {
        public bool IsStarted { get; private set; }
        private Player player;

        public Player Player => player;

        public void Init()
        {
            // Managers.cardDB로부터 임의의 덱을 섞어 만든다.
            player = new Player();
            IsStarted = false;
        }

        [ServerRpc]
        public void StartServerRpc()
        {
            Managers.Logger.Log<GameManager>("Game Start");
            Managers.Client.GameStartClientRpc();
        }
    }
}

