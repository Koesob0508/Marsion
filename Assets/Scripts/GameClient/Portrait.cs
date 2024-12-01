using UnityEngine;

namespace Marsion.Client
{
    public class Portrait : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer Player_Sprite;
        [SerializeField] private SpriteRenderer Enemy_Sprite;

        private void Start()
        {
            //Managers.Instance.Client.Game.OnGameStarted -= SetPortrait;
            //Managers.Instance.Client.Game.OnGameStarted += SetPortrait;

            Managers.Instance.Client.Game.OnGameStarted += SetPortrait;
        }

        private void SetPortrait()
        {
            foreach(var player in Managers.Instance.Client.Game.Data.Players)
            {
                // Player
                if(player.PlayerID == Managers.Instance.Client.Game.PlayerID)
                {
                    Player_Sprite.sprite = Managers.Instance.Data.PortraitDictionary[player.Portrait].Sprite;
                }
                // Enemy
                else
                {
                    Enemy_Sprite.sprite = Managers.Instance.Data.PortraitDictionary[player.Portrait].Sprite;
                }
            }
        }
    }
}