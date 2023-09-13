using ChickenScene.Entities;
using Optimization;
using UnityEngine;

namespace ChickenScene.Pools
{
    public class ChickenBasketPool: MonoBehaviour
    {
        [SerializeField] private ChickenLevelConfig _config;
        public PoolMono<ChickenBasket> Pool { get; private set; }

        private void Awake()
            => Pool = new PoolMono<ChickenBasket>(_config.ChickenBasket, 1, false, false, transform);
    }
}