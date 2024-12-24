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
            Managers.Instance.Client.Game.OnGameStarted -= ShowPanel;
            Managers.Instance.Client.Game.OnGameStarted += ShowPanel;
            Managers.Instance.Client.Game.OnManaChanged -= UpdateMana;
            Managers.Instance.Client.Game.OnManaChanged += UpdateMana;
        }

        private void ShowPanel()
        {
            Mana.gameObject.SetActive(true);
        }

        private void UpdateMana()
        {
            if (IsPlayer)
            {
                var player = Managers.Instance.Client.Game.Data.GetPlayer(Managers.Instance.Client.Game.PlayerID);
                Mana.text = $"{player.Mana}/{player.MaxMana}";
            }
            else
            {
                var player = Managers.Instance.Client.Game.Data.GetPlayer(Managers.Instance.Client.Game.EnemyID);
                Mana.text = $"{player.Mana}/{player.MaxMana}";
            }
        }
    }
}