using DG.Tweening;
using UnityEngine;

namespace Bee
{
    public class Cover : MonoBehaviour
    {
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _movingDuration;
        [SerializeField] private Transform _openedPosition;
        [SerializeField] private Transform _closePosition;

        // Close the object with a jumping animation
        public Tween Close()
        {
            return JumpTo(_closePosition.position);
        }

        // Open the object with a jumping animation
        public Tween Open()
        {
            return JumpTo(_openedPosition.position);
        }

        // Jump to a specific point with a given jump power and duration
        private Tween JumpTo(Vector3 point)
        {
            return transform.DOJump(point, _jumpPower, 1, _movingDuration);
        }
    }
}