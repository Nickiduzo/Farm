using Cinemachine;
using UnityEngine;
namespace Cameras
{
    /// <summary>
    /// Игровая камера, берёт параметры типа камеры и самой камеры
    /// </summary>
    public class GameCamera : MonoBehaviour
    {
        [SerializeField] private GameCameraType _type;
        [SerializeField] private CinemachineVirtualCamera _camera;

        public CinemachineVirtualCamera Camera => _camera;
        public GameCameraType Type => _type;
    }
}