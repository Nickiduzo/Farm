using Optimization;
using UnityEngine;

namespace CowScene.Spawners.Pools
{
    public class MilkShopPool : MonoBehaviour
    {
        public PoolMono<CowMilkShop> Pool { get; private set; }

        [SerializeField] private CowLevelConfig _config;

        //It creates a new object pool for CowMilkShop objects
        private void Awake()
            => Pool = new PoolMono<CowMilkShop>(_config.CowMilkShop,
                1,
                false, false,
                transform);
    }
}

