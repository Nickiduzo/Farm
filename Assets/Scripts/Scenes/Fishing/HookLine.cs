using UnityEngine;

namespace Fishing
{
    public class HookLine : MonoBehaviour
    {
        [SerializeField] private Transform _pivot;
        [SerializeField] private Transform _hook;

        private LineRenderer _lineRenderer;

        /// <summary>
        /// Запам'ятовуємо в полі "_lineRenderer" компонент "LineRenderer"
        /// </summary>
        void Start()
            => _lineRenderer = GetComponent<LineRenderer>();

        /// <summary>
        /// Викликає ф-цію "DrawHookLine"
        /// </summary>
        void Update() =>
            DrawHookLine();

        /// <summary>
        /// Малює позиції для гачку
        /// </summary>
        private void DrawHookLine()
        {
            _lineRenderer.SetPositions(new[]
            {
                _pivot.position, _hook.position
            });
        }

    }
}