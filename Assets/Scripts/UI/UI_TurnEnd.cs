using UnityEngine;
using UnityEngine.UI;

namespace Marsion.UI
{
    public class UI_TurnEnd : UI_Base
    {
        [SerializeField] Button button;

        public override void Init()
        {
            Managers.Instance.Client.Game.OnGameStarted -= ShowPanel;
            Managers.Instance.Client.Game.OnGameStarted += ShowPanel;
            Managers.Instance.Client.Game.OnTurnEnded -= ActivateButton;
            Managers.Instance.Client.Game.OnTurnEnded += ActivateButton;

            gameObject.SetActive(false);
        }

        private void ShowPanel()
        {
            gameObject.SetActive(true);

            ActivateButton();
        }

        private void ActivateButton()
        {
            if (Managers.Instance.Client.Game.IsMyTurn())
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
            Managers.Instance.Client.Game.SendTurnEnd();
        }
    }
}