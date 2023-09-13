using System;
using Tomato.Spawners;

namespace Tomato
{
    public class TomatoHolesContainer : HolesContainer<TomatoHole>
    {
        public event Action OnAllHolesReady;
        private int _holesReady;

        /// <summary>
        /// Додає події ямам "OnHoleReadyToSeed" ф-цію "CalculateReadyHole"
        /// </summary>
        private void Awake()
        {
            foreach (var hole in _holesOnScene)
                hole.OnHoleReadyToSeed += CalculateReadyHole;
        }

        /// <summary>
        /// Видаляє події ямам "OnHoleReadyToSeed" ф-цію "CalculateReadyHole"
        /// </summary>
        private void OnDestroy()
        {
            foreach (var hole in _holesOnScene)
                hole.OnHoleReadyToSeed -= CalculateReadyHole;
        }

        /// <summary>
        /// Рахує кількість готових ям [_holesReady], коли всі ями готові викликає подію "OnAllHolesReady"
        /// </summary>
        private void CalculateReadyHole()
        {
            _holesReady++;

            if (IsAllHolesReady())
                OnAllHolesReady?.Invoke();
        }

        /// <summary>
        /// Повертає значення чи всі ями готові 
        /// </summary>
        private bool IsAllHolesReady()
            => _holesReady >= _holesOnScene.Count;

        /// <summary>
        /// Дозволяє взаємодіяти з ямами [hole]
        /// </summary>
        public void MakeHolesInterable()
        {
            foreach (var hole in _holesOnScene)
                hole.MakeInteractable();
        }
    }
}