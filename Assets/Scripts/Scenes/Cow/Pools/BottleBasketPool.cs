using Optimization;
using UnityEngine;

namespace CowScene.Spawners.Pools
{
    public class BottleBasketPool : MonoBehaviour
    {
        public PoolMono<CollectionArea> Pool { get; private set; }

        [SerializeField] private CowLevelConfig _config;

        //It creates a new object pool for CollectionArea objects
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.CollectionArea,
                1,
                false, false,
                transform);
    }
}