using DG.Tweening;
using UnityEngine;

namespace Props
{
    [RequireComponent(typeof(Animator))]
    public class Butterfly : NPC
    {

        //NPS Move to target and wait
        public override void SetTarget(Transform target)
        {
            Target = target;
            int idleWait = Random.Range(2, 6);
            if (Target.position == this.transform.position)
                OnPosition();
            else
            {
                var sequence = DOTween.Sequence().SetLink(gameObject);
                sequence.AppendCallback(Fly);
                sequence.Append(transform.DORotate(RotationAngle(Target.position), 1f));
                sequence.Append(transform.DOMove(Target.position, 10f));
                sequence.AppendCallback(Idle);
                sequence.AppendInterval(idleWait);
                sequence.AppendCallback(OnPosition);
                sequence.Play();
            }
        }

        //Rotate NPS
        private Vector3 RotationAngle(Vector3 target)
        {
            var offsetRotation = new Vector3(0, 0, 90);
            Vector3 vectorToTarget = (target - transform.position).normalized;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
            return q.eulerAngles - offsetRotation;
        }
    }
}