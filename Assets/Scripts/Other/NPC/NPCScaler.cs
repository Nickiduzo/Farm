using System.Collections;
using UnityEngine;
/// <summary>
/// Класс для контроля размера NPS
/// </summary>
public class NPCScaler : MonoBehaviour
{
    [SerializeField] private Transform _NPCPivotTransform;

    private Coroutine _scaleCoroutine;
    private Camera _mainCamera;
    private const float MinScale = 0.45f;
    private Vector3 _startScale;
    /// <summary>
    /// Получает начальный размер и камеру
    /// </summary>
    private void Awake()
    {
        _startScale = transform.localScale;
        _mainCamera = Camera.main;
    }

    /// <summary>
    /// Начинает изменять размер
    /// </summary>
    public void StartScale()
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
        }
        _scaleCoroutine = StartCoroutine(ScaleNPC());
    }

    /// <summary>
    /// Завершает изменение размера
    /// </summary>
    public void StopScale()
    {
        if (_scaleCoroutine != null)
        {
            StopCoroutine(_scaleCoroutine);
            _scaleCoroutine = null;
        }
    }

    /// <summary>
    /// Сопрограмма для масштабирования NPC в зависимости от положения на экране
    /// </summary>
    /// <returns>задержка в фиксированном обновлении</returns>
    private IEnumerator ScaleNPC()
    {
        while (true)
        {
            var screenHeight = Screen.height;
            var screenPos = _mainCamera.WorldToScreenPoint(_NPCPivotTransform.position);
            var distanceFromTopNormalized = Mathf.InverseLerp(0f, screenHeight, screenHeight - screenPos.y);
            var scaledDistanceFromTop = Mathf.Lerp(MinScale, 1f, distanceFromTopNormalized);
            _NPCPivotTransform.localScale = _startScale * scaledDistanceFromTop;
            yield return new WaitForFixedUpdate();
        }
    }

}
