using TMPro;
using UnityEngine;

namespace Marsion.UI
{
    public class UI_Mana : UI_Base
    {
        [SerializeField] bool IsPlayer;
        [SerializeField] TMP_Text Mana;

        public override void Init()
        {
            Managers.Client.OnGameStarted -= ShowPanel;
            Managers.Client.OnGameStarted += ShowPanel;
            Managers.Client.OnTurnStarted -= UpdateMana;
            Managers.Client.OnTurnStarted += UpdateMana;
            Managers.Client.OnManaChanged -= UpdateMana;
            Managers.Client.OnManaChanged += UpdateMana;
        }

        private void ShowPanel()
        {
            Mana.gameObject.SetActive(true);

            UpdateMana();
        }

        private void UpdateMana()
        {
            if (IsPlayer)
            {
                var player = Managers.Client.GetGameData().GetPlayer(Managers.Client.ID);
                Mana.text = $"{player.Mana}/{player.MaxMana}";
            }
            else
            {
                var player = Managers.Client.GetGameData().GetPlayer(Managers.Client.EnemyID);
                Mana.text = $"{player.Mana}/{player.MaxMana}";
            }
        }
    }
}