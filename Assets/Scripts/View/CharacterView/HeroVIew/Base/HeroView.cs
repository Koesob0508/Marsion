using UnityEngine;

namespace Marsion.CardView
{
    public class HeroView : CharacterView, IHeroView
    {
        public override void Spawn()
        {
            FSM.PushState<CreatureViewSpawn>();
        }

        protected override void UpdateCard()
        {
            Card = Managers.Client.GetCard(Type, Card.PlayerID, Card.UID);
        }
    }
}