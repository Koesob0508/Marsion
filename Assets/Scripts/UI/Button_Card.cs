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
            if (Managers.Instance.Resource.GetDictionary<CardSO>().TryGetValue(soID, out var cardSO))
            {
                Text_Name.text = cardSO.Name;
                Text_Mana.text = cardSO.ManaCost.ToString();
                Text_AbilityExplain.text = cardSO.AbilityExplain;
                Text_Attack.text = cardSO.Attack.ToString();
                Text_Health.text = cardSO.Health.ToString();
                Image_Sprite.sprite = Managers.Instance.Resource.Load<Sprite>(cardSO.FullArtPath);
            }
            else
            {
                Logger.LogWarning<Button_Card>($"ID : {soID} CardSO not found", colorName: ColorCodes.ContentUI);
            }
        }
    }
}