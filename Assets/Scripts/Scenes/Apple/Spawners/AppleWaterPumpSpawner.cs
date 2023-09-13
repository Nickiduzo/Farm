using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;

namespace Apple
{
    public class AppleWaterPumpSpawner : MonoBehaviour
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private AppleWaterPumpPool _pool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private Transform _holePos;
        [SerializeField] private SoundSystem _soundSystem;

        private AppleWaterPump _pump;

        //Spawns water pump
        public AppleWaterPump SpawnWaterPump()
        {
            _pump = _pool.Pool.GetFreeElement();
            _pump.transform.position = _spawnPoint.position;
            _pump.Construct(_destinationPoint.position, _spawnPoint.position, _inputSystem, _holePos.position);
            _pump.GetComponentInChildren<WaterPumpStreamPrefab>().Construct(_soundSystem);
            return _pump;
        }
        
    }
}