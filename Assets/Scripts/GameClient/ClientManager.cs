using Marsion.Client;
using System.Collections.Generic;
using UnityEngine;

namespace Marsion
{
    public class ClientManager : MonoBehaviour
    {
        public GameClient Game;
        public GameClientEx GameEx;
        public DraftClient Draft;

        public InputManager Input { get; private set; }

        public ulong ID { get; private set; }

        public void Init()
        {
            Managers.Logger.Log<ClientManager>("Client Manager initialized", colorName: ColorCodes.Client);

            Input = new InputManager();
            Draft = new DraftClient();
            Draft.Init();
            //Game.Init();
            GameEx.Init();

            Managers.UI.ShowPopupUI<UI_Connect>();
        }

        private void Update()
        {
            Input.Update();
        }

        private void Clear()
        {

        }

        public void Ready(List<string> deck)
        {
            Game.Ready(deck);
        }
    }
}