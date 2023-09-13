namespace Cameras
{
    /// <summary>
    /// Интерфейс системы камеры, что имеет метод смены камеры
    /// </summary>
    public interface ICameraSystem
    {
        /// <summary>
        /// Смена камеры
        /// </summary>
        /// <param name="type">тип камеры</param>
        void ChangeCamera(GameCameraType type);
    }
}