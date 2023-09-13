using System;
using UnityEngine;

namespace Apple.Spawners
{
    public class SeedSpawner : MonoBehaviour
    {
        [SerializeField] private appleTest _config;

        public event Action<Seed> OnSeedSpawn;

        public Seed SpawnSeed(Vector3 at)
        {
            Seed seed = Instantiate(_config.Seed, at, Quaternion.identity);
            OnSeedSpawn?.Invoke(seed);
            return seed;
        }
    }
}