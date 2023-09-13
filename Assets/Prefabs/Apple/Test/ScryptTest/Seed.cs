using DG.Tweening;
using UnityEngine;

namespace Apple
{
    public class Seed : MonoBehaviour
    {
        public Tween MoveTo(Vector3 to)
            => transform.DOMove(to, 0.3f);
    }
}