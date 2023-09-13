using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using UsefulComponents;

namespace Props
{
    //скріпт що відповідає за поведінку бджоли
    public class Bee : NPC
    {
        // встановлює рандомний спот з NPCManager
        public override void SetTarget(Transform target)
        {
            Target = target;
            // повертає в сторону споту
            if ((transform.position - Target.position).x > 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            else
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

            // сигнал що добрався до позиції
            if (Target.position == this.transform.position)
                OnPosition();

            else
            {

                var sequence = DOTween.Sequence().SetLink(gameObject);

                // твін, рухає в бік споту , швидкість залежить від відстані, того стала
                sequence.Append(transform.DOMove(Target.position, Vector3.Distance(this.transform.position, Target.position)));
                sequence.AppendCallback(OnPosition);
            }
        }
    }
}
