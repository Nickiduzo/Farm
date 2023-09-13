using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;

namespace Carrot.Spawners
{
    public class WaterPumpSpawner : MonoBehaviour
    {
        private const float X_POS = 0.9f;
        private const float Y_POS = 0.25f;

        [SerializeField] private Camera _camera;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private WaterPumpPool _pool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;

        // get [WaterPump] from pool, set stawn point for it
        public WaterPump SpawnWaterPump()
        {
            WaterPump pump = _pool.Pool.GetFreeElement();
            pump.transform.position = _spawnPoint.position;
            CalculateDestinationPoint();
            pump.Construct(_destinationPoint.position, _spawnPoint.position, _inputSystem);
            pump.GetComponentInChildren<WaterPumpStreamPrefab>().Construct(_soundSystem);
            return pump;
        }

        // set destination point
        private void CalculateDestinationPoint()
            => _destinationPoint.position = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
    }
}