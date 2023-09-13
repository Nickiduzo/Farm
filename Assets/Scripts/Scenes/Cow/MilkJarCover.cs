using DG.Tweening;
using UnityEngine;

namespace CowScene
{
    public class MilkJarCover : MonoBehaviour
    {
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _movingDuration;
        [SerializeField] private Transform _openedPosition;
        [SerializeField] private Transform _closePosition;

        // Closes the object by jumping to the specified close position
        public Tween Close()
            => JumpTo(_closePosition.position);

        // Opens the object by jumping to the specified opened position
        public Tween Open()
            => JumpTo(_openedPosition.position);

        // Jumps to the specified point using DOTween
        private Tween JumpTo(Vector3 point)
            => transform.DOJump(point, _jumpPower, 1, _movingDuration);

    }
}