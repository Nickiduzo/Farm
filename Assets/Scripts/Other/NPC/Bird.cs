using DG.Tweening;
using UnityEngine;

namespace Props
{
    public class Bird : NPC
    {
        //NPS Move to target and wait
        public override void SetTarget(Transform target)
        {
            Target = target;
            transform.rotation = Quaternion.Euler(new Vector3(0, (transform.position - Target.position).x > 0 ? 0 : 180, 0));

            if (Target.position == transform.position)
            {
                OnPosition();
            }
            else
            {
                int idleWait = Random.Range(7, 13);
                var sequence = DOTween.Sequence().SetLink(gameObject);
                sequence.AppendCallback(Fly);
                sequence.Append(transform.DOMove(Target.position, Vector3.Distance(transform.position, Target.position) / 3f));
                sequence.AppendCallback(Idle);
                sequence.AppendInterval(idleWait);
                sequence.AppendCallback(OnPosition);
                sequence.Play();
            }
        }
    }
}