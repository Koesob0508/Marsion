using UnityEngine;
using UnityEngine.Events;

namespace Marsion.CardView
{
    public class CharacterViewFSM : BaseStateMachine
    {
        private new ICharacterView Handler { get; }
        private CreatureViewSpawn SpawnState { get; }
        private CreatureViewIdle IdleState { get; }
        private CreatureViewSelect SelectState { get; }
        private CreatureViewAttack AttackState { get; }
        private CreatureViewDead DeadState { get; }

        public GameObject Target;

        public CharacterViewFSM(ICharacterView handler = null) : base(handler)
        {
            Handler = handler;

            SpawnState = new CreatureViewSpawn(handler, this);
            IdleState = new CreatureViewIdle(handler, this);
            SelectState = new CreatureViewSelect(handler, this);
            AttackState = new CreatureViewAttack(handler, this);
            DeadState = new CreatureViewDead(handler, this);

            RegisterState(SpawnState);
            RegisterState(IdleState);
            RegisterState(SelectState);
            RegisterState(AttackState);
            RegisterState(DeadState);

            Initialize();
        }

        public void ActivatePointer(string layerName, UnityAction<bool, GameObject> action)
        {
            Handler.Pointer.gameObject.SetActive(true);

            Handler.Pointer.LayerName = layerName;
            Handler.Pointer.OnClick -= action;
            Handler.Pointer.OnClick += action;
        }

        public void DeactivatePointer()
        {
            Handler.Pointer.transform.localPosition = Vector3.zero;
        }
    }
}