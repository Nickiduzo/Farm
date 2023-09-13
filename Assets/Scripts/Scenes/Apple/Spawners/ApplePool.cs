using Optimization;
using UnityEngine;

namespace Apple
{
    public class ApplePool : MonoBehaviour
    {
        public PoolMono<AppleFruit> Pool { get; private set; }

        [SerializeField] private AppleLevelConfig _config;

        //It creates a new object pool for AppleFruit objects
        private void Awake()
            => Pool = new PoolMono<AppleFruit>(_config.Apple,
                _config.ApplesToSpawn,
                false, false,
                transform);
    }
}