using UnityEngine;
using UnityEngine.Events;

namespace Marsion.CardView
{
    public class CreatureViewFSM : BaseStateMachine
    {
        private new ICreatureView Handler { get; }
        private CreatureViewSpawn SpawnState { get; }
        private CreatureViewIdle IdleState { get; }
        private CreatureViewSelect SelectState { get; }

        public CreatureViewFSM(ICreatureView handler = null)
        {
            Handler = handler;

            SpawnState = new CreatureViewSpawn(handler, this);
            IdleState = new CreatureViewIdle(handler, this);
            SelectState = new CreatureViewSelect(handler, this);

            RegisterState(SpawnState);
            RegisterState(IdleState);
            RegisterState(SelectState);

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