using DG.Tweening;
using UnityEngine;
using System;

namespace Tomato
{
    public class WormJarCover : MonoBehaviour
    {
        [SerializeField] private float _jumpPower;
        [SerializeField] private float _movingDuration;
        [SerializeField] private Transform _openedPosition;
        [SerializeField] private Transform _closePosition;

        public Action OnClosed;
        public Action OnOpened;

        /// <summary>
        /// Переміщує стрибком до позиції закриття [_closePosition]
        /// </summary>
        public Tween Close()
            => JumpTo(_closePosition.position).OnComplete(()=> 
                {
                    OnClosed?.Invoke();
                });

        /// <summary>
        /// Переміщує стрибком до позиції відкриття [_openedPosition]
        /// </summary>
        public Tween Open()
            => JumpTo(_openedPosition.position).OnComplete(()=> 
                {
                    OnOpened?.Invoke();
                });

        /// <summary>
        /// Вводимо позицію [point] - переміщує стрибком до позиції елемент
        /// </summary>
        private Tween JumpTo(Vector3 point)
            => transform.DOJump(point, _jumpPower, 1, _movingDuration);
    }
}