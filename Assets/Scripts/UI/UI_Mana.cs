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
            Managers.Client.Game.OnGameStarted -= ShowPanel;
            Managers.Client.Game.OnGameStarted += ShowPanel;
            Managers.Client.Game.OnTurnStarted -= UpdateMana;
            Managers.Client.Game.OnTurnStarted += UpdateMana;
            Managers.Client.Game.OnManaChanged -= UpdateMana;
            Managers.Client.Game.OnManaChanged += UpdateMana;
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
                var player = Managers.Client.Game.GetGameData().GetPlayer(Managers.Client.ID);
                Mana.text = $"{player.Mana}/{player.MaxMana}";
            }
            else
            {
                var player = Managers.Client.Game.GetGameData().GetPlayer(Managers.Client.Game.EnemyID);
                Mana.text = $"{player.Mana}/{player.MaxMana}";
            }
        }
    }
}