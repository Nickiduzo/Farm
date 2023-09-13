using AwesomeTools.Inputs;
using Sheep.Spawners.Pools;
using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace Sheep.Spawners
{
    public class TrimmerSpawner : MonoBehaviour
    {
        public event Action<Trimmer> OnTrimmerSpawned;

        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private Transform _spawnPosition;
        [SerializeField] private Transform _startPosition;
        [SerializeField] private TrimmerPool _trimmerPool;
        public Trimmer SpawnedTrimmer { get; private set; }

        //Spawns trimmer
        public void SpawnTrimmer(FxSystem fxSystem)
        {
            Trimmer trimmer = _trimmerPool.Pool.GetFreeElement();
            trimmer.transform.position = _spawnPosition.position;

            OnTrimmerSpawned?.Invoke(trimmer);

            trimmer.Construct(_startPosition.position, _spawnPosition.position, _inputSystem, _soundSystem, fxSystem);

            SpawnedTrimmer = trimmer;
        }
    }
}