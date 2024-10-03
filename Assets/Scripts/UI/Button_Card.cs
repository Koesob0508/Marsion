using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marsion.UI
{
    public class Button_Card : MonoBehaviour
    {
        [SerializeField] TMP_Text Text_Name;
        [SerializeField] TMP_Text Text_Mana;
        [SerializeField] TMP_Text Text_AbilityExplain;
        [SerializeField] TMP_Text Text_Attack;
        [SerializeField] TMP_Text Text_Health;
        [SerializeField] Image Image_Sprite;

        public Button Button;

        public void Setup(Card card)
        {
            Text_Name.text = card.Name;
            Text_Mana.text = card.Mana.ToString();
            Text_AbilityExplain.text = card.AbilityExplain;
            Text_Attack.text = card.Attack.ToString();
            Text_Health.text = card.Health.ToString();
            Image_Sprite.sprite = Managers.Resource.Load<Sprite>(card.FullArtPath);
        }
    }
}