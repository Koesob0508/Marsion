using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marsion.UI
{
    public class Content_Card : UI_Base
    {
        public int Cost;
        public int Count = 0;
        public TMP_Text Text_Name;
        [SerializeField] TMP_Text Text_Cost;
        [SerializeField] TMP_Text Text_Count;
        [SerializeField] Image Image_Card;

        public override void Init()
        {
            
        }

        public void Setup(string soID)
        {
            if (Managers.Data.CardDictionary.TryGetValue(soID, out var cardSO))
            {
                Text_Name.text = cardSO.Name;
                Cost = cardSO.Mana;
                Text_Cost.text = cardSO.Mana.ToString();
                Image_Card.sprite = Managers.Resource.Load<Sprite>(cardSO.BoardArtPath);
            }
            else
            {
                Managers.Logger.LogWarning<Content_Card>($"ID : {soID} CardSO not found", colorName: ColorCodes.ContentUI);
            }
        }

        public void IncreaseCount()
        {
            Count++;
            Text_Count.gameObject.SetActive(true);
            Text_Count.text = Count.ToString();
        }

        public bool DecreaseCount()
        {
            Count--;
            Text_Count.gameObject.SetActive(true);
            Text_Count.text = Count.ToString();

            if(Count <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}