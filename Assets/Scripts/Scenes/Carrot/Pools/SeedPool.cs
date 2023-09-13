using Carrot.Config;
using Optimization;
using UnityEngine;

namespace Carrot.Spawners
{
    public class SeedPool : MonoBehaviour
    {

        [SerializeField] private CarrotLevelConfig _config;
        public PoolMono<Seed> Pool { get; private set; }

        // get [Seed] from config for pool, get number of seeds that will appear
        private void Awake()
            => Pool = new PoolMono<Seed>(_config.Seed, CalculateSeedCount(), true, true, transform);

        // set seeds count
        private int CalculateSeedCount()
            => (_config.MaxMoleToSpawn + 1) * 5;
    }
}