using Carrot.Config;
using Optimization;
using UnityEngine;

namespace Carrot.Spawners
{
    public class SeedPackagePool : MonoBehaviour
    {

        [SerializeField] private CarrotLevelConfig _config;
        public PoolMono<SeedPackage> Pool { get; private set; }

        // get [SeedPackage] from config for pool
        private void Awake()
            => Pool = new PoolMono<SeedPackage>(_config.SeedPackage, 1, false, false, transform);
    }
}