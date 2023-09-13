using System;
using UnityEngine;

namespace Carrot.Spawners
{
    public class SeedSpawner : MonoBehaviour
    {
        [SerializeField] private SeedPool _pool;
        public event Action<Seed> OnSeedSpawn;

        // get [Seed] from pool, set position fro seed, invoke Action [OnSeedSpawn] which pass [Seed]
        public Seed SpawnSeed(Vector3 at)
        {
            Seed seed = _pool.Pool.GetFreeElement();
            seed.transform.position = at;
            OnSeedSpawn?.Invoke(seed);
            return seed;
        }
    }
}