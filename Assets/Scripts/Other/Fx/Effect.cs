using DG.Tweening;
using UnityEngine;

namespace Fx
{
    /// <summary>
    /// Класс эффекта и его визуальной частью на сцене
    /// </summary>
    public class Effect : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private float _appearDuration;
        [SerializeField] private float _selfDestroyInterval;

        private bool isPlaying = false;

        /// <summary>
        /// Последовательность жизни эффекта в виде размера и спрайта
        /// </summary>
        public void Appear()
        {
            AppearWithScale();
            AppearWithSprite();
        }
        /// <summary>
        /// Воспроизведение эффекта с параметрами
        /// </summary>
        public void Play()
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            isPlaying = true;
            var sequence = DOTween.Sequence();
            sequence.Append(_sprite.DOFade(1, _appearDuration));
            sequence.AppendInterval(_selfDestroyInterval);
            sequence.Append(_sprite.DOFade(0, _appearDuration));
            sequence.AppendCallback(Reset);
        }
        /// <summary>
        /// Сбросить воспроизведение
        /// </summary>
        private void Reset() => isPlaying = false;
        /// <summary>
        /// Появление эффекта с спрайтом и затуханием
        /// </summary>
        private void AppearWithSprite()
        {
            gameObject.SetActive(true);
            var sequence = DOTween.Sequence();
            sequence.Append(_sprite.DOFade(1, _appearDuration));
            sequence.AppendInterval(_selfDestroyInterval);
            sequence.Append(_sprite.DOFade(0, _appearDuration));
            sequence.AppendCallback(Disable);
        }
        /// <summary>
        /// Появление эффекта с размером
        /// </summary>
        private void AppearWithScale()
        {
            Vector3 cachedScale = transform.localScale;
            transform.localScale = Vector3.zero;

            float random = GetRandomScaleMultiplier();

            transform.DOScale(cachedScale * random, _appearDuration);
        }
        /// <summary>
        /// Получение рандомного значения размера
        /// </summary>
        /// <returns>возвращает рандомное значение</returns>
        private float GetRandomScaleMultiplier()
            => Random.Range(0.3f, 1f);
        /// <summary>
        /// Отмена эффекта с его отключением
        /// </summary>
        private void Disable()
            => gameObject.SetActive(false);
    }
}