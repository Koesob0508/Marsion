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

        public void Setup(string soID)
        {
            if (Managers.Data.CardDictionary.TryGetValue(soID, out var cardSO))
            {
                Text_Name.text = cardSO.Name;
                Text_Mana.text = cardSO.Mana.ToString();
                Text_AbilityExplain.text = cardSO.AbilityExplain;
                Text_Attack.text = cardSO.Attack.ToString();
                Text_Health.text = cardSO.Health.ToString();
                Image_Sprite.sprite = Managers.Resource.Load<Sprite>(cardSO.FullArtPath);
            }
            else
            {
                Managers.Logger.LogWarning<Button_Card>($"ID : {soID} CardSO not found", colorName: ColorCodes.ContentUI);
            }
        }
    }
}