using Marsion.Client;
using UnityEngine;

namespace Marsion
{
    public class ClientManager : MonoBehaviour, IClientManager
    {
        //public GameClient Game;
        [SerializeField] GameClientEx game;

        public IGameClient Game => game;
        public DraftClient Draft { get; private set; }

        public InputManager Input { get; private set; }

        public void Init()
        {
            Logger.Log<ClientManager>("Client Manager initialized", colorName: ColorCodes.Client);

            Input = new InputManager();
            Draft = new DraftClient(Managers.Instance);
            Draft.Init();
            //Game.Init();
            Game.Init();

            Managers.Instance.UI.ShowPopupUI<UI_Connect>();
        }

        private void Update()
        {
            Input.Update();
        }

        private void Clear()
        {

        }
    }
}