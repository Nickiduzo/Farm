using Optimization;
using UnityEngine;

namespace Apple.Spawners.Pools
{
    public class SeedlingsApplePool : MonoBehaviour
    {
        public PoolMono<SeedlingsApple> Pool { get; private set; }

        [SerializeField] private AppleLevelConfig _config;

        //It creates a new object pool for SeedlingsApple objects
        private void Awake()
            => Pool = new PoolMono<SeedlingsApple>(_config.SeedlingsApple,
                1,
                false, false,
                transform);
    }
}


