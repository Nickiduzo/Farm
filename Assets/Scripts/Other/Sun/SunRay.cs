using DG.Tweening;
using UnityEngine;

/// <summary>
/// Класс солнечных лучей
/// </summary>
public class SunRay : MonoBehaviour
{
    [SerializeField] private Vector3 _scaleValue;
    [SerializeField] private float _minDuration;
    [SerializeField] private float _maxDuration;

    /// <summary>
    /// Запуск анимации при старте
    /// </summary>
    private void Awake()
    {
        Shine();
    }

    /// <summary>
    /// Бесконечная анимация с контролем размера
    /// </summary>
    private void Shine()
    {
        transform.DOScale(_scaleValue, Random.Range(_minDuration, _maxDuration))
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(1f).SetLink(transform.gameObject)
            .OnComplete(Shine);
    }
}