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

            Managers.Client.GameEx.OnGameStarted += SetPortrait;
        }

        private void SetPortrait()
        {
            foreach(var player in Managers.Client.GameEx.Data.Players)
            {
                // Player
                if(player.ClientID == Managers.Client.ID)
                {
                    //Player_Sprite.sprite = Managers.Data.PortraitDictionary[player.Portrait].Sprite;
                }
                // Enemy
                else
                {
                    //Enemy_Sprite.sprite = Managers.Data.PortraitList[player.Portrait].Sprite;
                }
            }
        }
    }
}