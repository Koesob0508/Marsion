using DG.Tweening;
using UnityEngine;

namespace Marsion.CardView
{
    public class CreatureViewAttack : BaseCreatureViewState
    {
        Sequence attackSequence;
        Sequence backSequence;
        Vector3 DefaultSize = Vector3.one;
        Vector3 OriginalPosition;
        Quaternion OriginalRotation;

        public CreatureViewAttack(ICreatureView handler, CreatureViewFSM fsm) : base(handler, fsm) { }

        public override void OnEnterState()
        {
            Handler.Order.SetMostFrontOrder(true);
            OriginalPosition = Handler.Transform.position;
            OriginalRotation = Handler.Transform.rotation;

            Vector3 directionToTarget = (FSM.Target.transform.position - (Handler.Transform.position + new Vector3(0f, 0f, -2f))).normalized;

            Quaternion targetRotation;

            if(Managers.Client.IsMine(Handler.Card))
                targetRotation = Quaternion.FromToRotation(Vector3.up, directionToTarget);
            else
                targetRotation = Quaternion.FromToRotation(Vector3.down, directionToTarget);

            attackSequence = DOTween.Sequence().Pause()
                .Append(Handler.Transform.DOMove(Handler.Transform.position + new Vector3(0f, 0f, -2f), 0.5f).SetEase(Ease.InCubic))
                .Append(Handler.Transform.DORotateQuaternion(targetRotation, 0.2f).SetEase(Ease.InOutQuad))
                .Append(Handler.Transform.DOMove(FSM.Target.transform.position, 0.2f).SetEase(Ease.InExpo))
                .OnComplete(() =>
                {
                    Handler.UpdateStatus();
                    FSM.Target.GetComponent<ICreatureView>().UpdateStatus();
                    backSequence.Play();
                });
                

            backSequence = DOTween.Sequence().Pause()
                .Append(Handler.Transform.DOMove(OriginalPosition, 0.3f).SetEase(Ease.InExpo))
                .Join(Handler.Transform.DORotateQuaternion(OriginalRotation, 0.3f).SetEase(Ease.InExpo))
                .OnComplete(() =>
                {
                    Handler.Order.SetMostFrontOrder(false);
                    FSM.PopState();
                });

            attackSequence.Play();
        }

        public override void OnExitState()
        {
            
        }
    }
}