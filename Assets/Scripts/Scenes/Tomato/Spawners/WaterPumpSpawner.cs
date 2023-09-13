using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;

namespace Tomato.Spawners
{
    public class WaterPumpSpawner : MonoBehaviour
    {
        private const float X_POS = 0.9f;
        private const float Y_POS = 0.15f;
        [SerializeField] private Camera _camera;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private WaterPumpPool _pool;

        private Vector3 _destinationPoint;

        /// <summary>
        /// Викликає лійку
        /// </summary>        
        public WaterPump SpawnWaterPump()
        {
            WaterPump pump = _pool.Pool.GetFreeElement();
            pump.transform.position = _spawnPoint.position;
            CalculateDestinationPoint();
            pump.Construct(_destinationPoint, _spawnPoint.position, _inputSystem);
            pump.GetComponentInChildren<WaterPumpStreamPrefab>().Construct(_soundSystem);
            return pump;
        }

        /// <summary>
        /// Рахує позицію призначення [_destinationPoint]
        /// </summary>
        private void CalculateDestinationPoint()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _destinationPoint = new Vector3(destination.x, destination.y, destination.z);
        }

        /// <summary>
        /// Повертає позицію призначення [_destinationPoint]
        /// </summary>        
        public Vector3 GetDestinationPoint()
            => _destinationPoint;
    }
}