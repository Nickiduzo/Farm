using Optimization;
using UnityEngine;

namespace Sheep.Spawners.Pools
{
    public class ShopPool : MonoBehaviour
    {
        public PoolMono<Shop> Pool { get; private set; }

        [SerializeField] private SheepLevelConfig _config;

        //It creates a new object pool for Shop objects
        private void Awake()
            => Pool = new PoolMono<Shop>(_config.Shop,
                1,
                false, false,
                transform);
    }
}