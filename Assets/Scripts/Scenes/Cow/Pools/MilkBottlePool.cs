using Optimization;
using UnityEngine;

namespace CowScene.Spawners.Pools
{
    public class MilkBottlePool : MonoBehaviour
    {
        public PoolMono<MilkBottle> Pool { get; private set; }

        [SerializeField] private CowLevelConfig _config;

        //It creates a new object pool for MilkBottle objects
        private void Awake()
            => Pool = new PoolMono<MilkBottle>(_config.MilkBottle,
                _config.BottleCount,
                false, false,
                transform);
    }
}

