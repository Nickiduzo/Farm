using Optimization;
using UnityEngine;

namespace Apple.Spawners
{
    public class SeedPackagePool : MonoBehaviour
    {

        [SerializeField] private appleTest _config;
        public PoolMono<SeedPackage> Pool { get; private set; }

        private void Awake()
            => Pool = new PoolMono<SeedPackage>(_config.SeedPackage, 1, false, false, transform);
    }
}