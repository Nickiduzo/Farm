using Optimization;
using UnityEngine;

namespace CowScene.Spawners.Pools
{
    public class HayPool : MonoBehaviour
    {
        public PoolMono<Hay> Pool { get; private set; }

        [SerializeField] private CowLevelConfig _config;

        //It creates a new object pool for Hay objects
        private void Awake()
            => Pool = new PoolMono<Hay>(_config.Hay,
                _config.HayCount,
                false, false,
                transform);
    }
}

