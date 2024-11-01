using UnityEngine;

namespace Marsion
{
    [CreateAssetMenu(fileName = "PortraitSO", menuName = "Marsion/PortraitSO")]
    public class PortraitSO : ScriptableObject, IIdentifiable
    {
        [SerializeField] private string id;
        public string ID => id;
        public Sprite Sprite;
    }
}