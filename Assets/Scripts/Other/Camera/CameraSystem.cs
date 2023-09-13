using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
    /// <summary>
    /// Система контроля приоритета камер
    /// </summary>
    public class CameraSystem : MonoBehaviour, ICameraSystem
    {
        [SerializeField] private List<GameCamera> _cameras;
        /// <summary>
        /// Изменение приоритета камер на низкую, а указанный тип выше
        /// </summary>
        /// <param name="type">тип игровой камеры</param>
        public void ChangeCamera(GameCameraType type)
        {
            foreach (var gameCamera in _cameras)
            {
                gameCamera.Camera.Priority = 0;

                if (gameCamera.Type == type)
                    gameCamera.Camera.Priority = 1;
            }
        }
    }
}