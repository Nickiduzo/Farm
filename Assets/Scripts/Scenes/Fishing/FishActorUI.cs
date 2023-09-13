using System;
using Fishing;

namespace UI
{
    /// <summary>
    /// Класс актёра с событиями
    /// </summary>
    public class FishActorUI : ActorUI
    {
        public static event Action OnUpdateFishCounter;
        public static event Action OnWithDrawFish;
        public static event Action OnAppearFishCounter;

        /// <summary>
        /// Подписка на событие
        /// </summary>
        /// <param name="progressWriter">интерфейс прогресса</param>
        public override void InitProgressBar(IProgressWriter progressWriter)
        {
            base.InitProgressBar(progressWriter);
            _progressWriter.OnProgressChanged += UpdateFishCounter;
        }
        /// <summary>
        /// Отписка от событий
        /// </summary>
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _progressWriter.OnProgressChanged -= UpdateFishCounter;
        }
        /// <summary>
        /// Вызов события класса, если он есть
        /// </summary>
        private void UpdateFishCounter() => OnUpdateFishCounter?.Invoke();
        /// <summary>
        /// Вызов события класса, если он есть
        /// </summary>
        /// <param name="fish">класс рыбы</param>
        public void WithDrawFish(Fish fish) => OnWithDrawFish?.Invoke();
        /// <summary>
        /// Вызов события класса, если он есть
        /// </summary>
        public void AppearFishCounter() => OnAppearFishCounter?.Invoke();
    }
}