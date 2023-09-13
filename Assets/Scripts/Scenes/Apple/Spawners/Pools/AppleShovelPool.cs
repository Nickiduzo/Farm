using Optimization;
using UnityEngine;

namespace Apple
{
    public class AppleShovelPool : MonoBehaviour
    {
        public PoolMono<AppleShovel> Pool { get; private set; }

        [SerializeField] private AppleLevelConfig _config;

        //It creates a new object pool for AppleShovel objects
        private void Awake()
            => Pool = new PoolMono<AppleShovel>(_config.Shovel,
                1,
                false, false,
                transform);
    }
}