using UnityEngine;

namespace Marsion.CardView
{
    public class HeroView : CharacterView, IHeroView
    {
        public override void Spawn()
        {
            FSM.PushState<CreatureViewSpawn>();
        }

        public override void Die()
        {
            FSM.PushState<CreatureViewDead>();
        }

        protected override void UpdateCard()
        {
            Card = Managers.Instance.Client.Game.GetCard(Type, Card.PlayerID, Card.UID);
        }
    }
}