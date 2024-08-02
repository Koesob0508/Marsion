using UnityEngine;

namespace Marsion.Client
{
    public class InputManager
    {
        public void Update()
        {
#if UNITY_EDITOR
            InputCheatKey();
#endif
        }

        private void InputCheatKey()
        {
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                if (!Managers.Server.IsConnected) return;
                Managers.Logger.Log<InputManager>("Server connected.");
            }
        }
    }
}