using UnityEngine;

namespace Marsion.Client
{
    public class Portrait : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer Player_Sprite;
        [SerializeField] private SpriteRenderer Enemy_Sprite;

        private void Start()
        {
            //Managers.Client.Game.OnGameStarted -= SetPortrait;
            //Managers.Client.Game.OnGameStarted += SetPortrait;

            Managers.Client.Game.OnGameStarted += SetPortrait;
        }

        private void SetPortrait()
        {
            foreach(var player in Managers.Client.Game.Data.Players)
            {
                // Player
                if(player.PlayerID == Managers.Client.Game.PlayerID)
                {
                    Player_Sprite.sprite = Managers.Instance.Data.GetDictionary<PortraitSO>()[player.Portrait].Sprite;
                }
                // Enemy
                else
                {
                    Enemy_Sprite.sprite = Managers.Instance.Data.GetDictionary<PortraitSO>()[player.Portrait].Sprite;
                }
            }
        }
    }
}