using Marsion;
using UnityEngine;

namespace Marsion.Client
{
    public class InputManager
    {
        public void Init()
        {

        }

        public void Update()
        {
            InputKey();
        }

        private void InputKey()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Managers.Client.GetField();
                Managers.Client.GetHand();
            }

            if(Input.GetKeyDown(KeyCode.Keypad0))
            {
                Managers.Client.PlayCard(0);
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                Managers.Client.PlayCard(1);
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                Managers.Client.PlayCard(2);
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                Managers.Client.PlayCard(3);
            }
        }
    }
}