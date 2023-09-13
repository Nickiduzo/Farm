using DG.Tweening;
using UnityEngine;

/// <summary>
/// Вращение солнечных лучей
/// </summary>
public class SunRaysRotator : MonoBehaviour
{
    [SerializeField] private Transform _sunRaysCointainer;
    [SerializeField] private float _rotateDuration;

    /// <summary>
    /// Запуск метода с вращением в начале игры
    /// </summary>
    private void Awake()
    {
        RotateRays();
    }

    /// <summary>
    /// Зацикленный класс вращения лучей
    /// </summary>
    private void RotateRays()
    {
        _sunRaysCointainer.DORotate(new Vector3(0, 0, 360), _rotateDuration, RotateMode.FastBeyond360)
            .SetLink(_sunRaysCointainer.gameObject)
            .OnComplete(RotateRays);
    }
}