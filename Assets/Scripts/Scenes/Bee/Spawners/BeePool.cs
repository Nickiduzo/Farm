using Bee.Config;
using Optimization;
using UnityEngine;

namespace Bee.Spawners
{
    public class BeePool : MonoBehaviour
    {
        [SerializeField] private BeeLevelConfig _config;
        public PoolMono<Bee> Pool { get; private set; }

        //It creates a new object pool for Bee objects
        private void Awake()
            => Pool = new PoolMono<Bee>(_config.Bee, _config.BeeToSpawn, false, false, transform);
    }
}