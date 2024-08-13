using UnityEngine;

namespace Marsion.Client
{
    public class Portrait : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer Player_Sprite;
        [SerializeField] private SpriteRenderer Enemy_Sprite;

        private void Start()
        {
            Managers.Client.OnGameStarted -= SetPortrait;
            Managers.Client.OnGameStarted += SetPortrait;
        }

        private void SetPortrait()
        {
            if(Managers.Network.IsHost)
            {
                Player_Sprite.sprite = Managers.Client.PortraitSprites[0];
                Enemy_Sprite.sprite = Managers.Client.PortraitSprites[1];
            }
            else
            {
                Player_Sprite.sprite = Managers.Client.PortraitSprites[1];
                Enemy_Sprite.sprite = Managers.Client.PortraitSprites[0];
            }
        }
    }
}