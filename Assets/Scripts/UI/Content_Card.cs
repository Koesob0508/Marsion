using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Marsion.UI
{
    public class Content_Card : UI_Base
    {
        public int Cost;
        public int Count = 0;
        [SerializeField] TMP_Text Text_Name;
        [SerializeField] TMP_Text Text_Cost;
        [SerializeField] TMP_Text Text_Count;
        [SerializeField] Image Image_Card;

        public override void Init()
        {
            
        }

        public void Setup(Card card)
        {
            Text_Name.text = card.Name;
            Cost = card.Mana;
            Text_Cost.text = card.Mana.ToString();
            Image_Card.sprite = Managers.Resource.Load<Sprite>(card.BoardArtPath);
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