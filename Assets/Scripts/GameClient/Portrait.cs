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
            foreach(var player in Managers.Client.GetGameData().Players)
            {
                // Player
                if(player.ClientID == Managers.Client.ID)
                {
                    Player_Sprite.sprite = Managers.Client.GetPortrait(player.Portrait);
                }
                // Enemy
                else
                {
                    Enemy_Sprite.sprite = Managers.Client.GetPortrait(player.Portrait);
                }
            }
        }
    }
}