using System;

namespace UsefulComponents
{
    /// <summary>
    /// класс для контроля прогресса 
    /// </summary>
    public class FrameProgressCalculator : IProgressCalculator
    {
        private float _currentProgress;
        private float _maxProgress;

        public event Action OnProgressStep;

        /// <summary>
        /// установить [_maxProgress] для некоторого действия
        /// </summary>
        /// <param name="maxProgress">максимальное значение прогресса</param>
        public FrameProgressCalculator(float maxProgress)
            => _maxProgress = maxProgress;

        /// <summary>
        /// обновление прогресса на основе [deltaTime]
        /// </summary>
        /// <param name="deltaTime">параметр, что заполняет прогресс за кадр</param>
        public void AddProgress(float deltaTime)
        {
            _currentProgress += deltaTime;

            if (!IsFullProgress()) return;

            OnProgressStep?.Invoke();
            _currentProgress = 0;
        }

        /// <summary>
        /// проверяет, является ли текущий прогресс максимальным, если да, верните «true»
        /// </summary>
        /// <returns>возвращает максимальный прогресс</returns>
        private bool IsFullProgress()
            => _currentProgress >= _maxProgress;
    }
}