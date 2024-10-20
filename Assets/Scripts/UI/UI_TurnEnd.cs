using UnityEngine;
using UnityEngine.UI;

namespace Marsion.UI
{
    public class UI_TurnEnd : UI_Base
    {
        [SerializeField] Button button;

        public override void Init()
        {
            Managers.Client.Game.OnGameStarted -= ShowPanel;
            Managers.Client.Game.OnGameStarted += ShowPanel;
            Managers.Client.Game.OnTurnEnded -= ActivateButton;
            Managers.Client.Game.OnTurnEnded += ActivateButton;

            gameObject.SetActive(false);
        }

        private void ShowPanel()
        {
            gameObject.SetActive(true);

            ActivateButton();
        }

        private void ActivateButton()
        {
            if (Managers.Client.Game.IsMyTurn())
            {
                button.gameObject.SetActive(true);
            }
            else
            {
                button.gameObject.SetActive(false);
            }
        }

        public void TurnEnd()
        {
            Managers.Client.Game.TurnEnd();
        }
    }
}