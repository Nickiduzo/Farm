using Bee.Config;
using Optimization;
using UnityEngine;

namespace Bee.Spawners
{
    public class HoneyGlassBasketPool : MonoBehaviour
    {
        [SerializeField] private BeeLevelConfig _config;
        public PoolMono<CollectionArea> Pool { get; private set; }

        //It creates a new object pool for CollectionArea objects
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.Basket, 1, false, false, transform);
    }
}