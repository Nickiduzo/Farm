using DG.Tweening;
using UnityEngine;

namespace UsefulComponents
{
    /// <summary>
    /// Класс с изменением размера в виде анимации Tween
    /// </summary>
    public class AppearAndDisappear : MonoBehaviour
    {

        [SerializeField] private float _appearDuration;
        private Vector3 _cachedScale;
        /// <summary>
        /// При старте сцены получает локальный размер объекта на котором расположен
        /// </summary>
        private void Awake()
            => _cachedScale = transform.localScale;
        
        /// <summary>
        /// функция Tween с увеличением объекта
        /// </summary>
        /// <returns>возвращает размер полученный при старте в анимацию Tween</returns>
        public Tween Appear()
            => transform.DOScale(_cachedScale, _appearDuration);
        
        /// <summary>
        /// функция Tween с уменьшением объекта
        /// </summary>
        /// <returns>возвращает нулевой размер в анимацию Tween</returns>
        public Tween Disappear()
            => transform.DOScale(Vector3.zero, _appearDuration);
    }
}