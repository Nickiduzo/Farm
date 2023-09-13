using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace Apple
{
    public class AppleFertilizerPackageSpawner : MonoBehaviour
    {
        public event Action OnPackageSpawned;

        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private FxSystem _fxSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private AppleFertilizerPool _pool;
        [SerializeField] private Transform _spawnPosition;
        [SerializeField] private Transform _startPosition;
        public AppleFertilizerPackage Package { get; private set; }

        //Spawns fertilizer
        public AppleFertilizerPackage SpawnFertilizerPackage(AppleHole hole)
        {
            AppleFertilizerPackage package = _pool.Pool.GetFreeElement();
            package.transform.position = _spawnPosition.position;

            OnPackageSpawned?.Invoke();

            package.Construct(_startPosition.position, _spawnPosition.position, _inputSystem, hole, _soundSystem, _fxSystem);

            Package = package;
            return Package;
        }
    }
}