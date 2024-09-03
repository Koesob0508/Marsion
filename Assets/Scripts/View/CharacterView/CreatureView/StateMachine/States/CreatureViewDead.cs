using DG.Tweening;
using UnityEngine;

namespace Marsion.CardView
{
    public class CreatureViewDead : BaseCreatureViewState
    {
        public CreatureViewDead(ICharacterView handler, CharacterViewFSM fsm) : base(handler, fsm) { }

        public override void OnEnterState()
        {
            Handler.Transform.DOShakeRotation(2f, strength: new Vector3(0f, 30f, 30f), vibrato: 3, randomness: 0)
                .OnComplete(() =>
                {
                    FSM.PopState();
                    Handler.MonoBehaviour.gameObject.SetActive(false);
                    OnComplete?.Invoke();
                    OnComplete = null;
                });
        }
    }
}