using Optimization;
using UnityEngine;

namespace Sheep.Spawners.Pools
{
    public class FurBasketPool : MonoBehaviour
    {
        public PoolMono<CollectionArea> Pool { get; private set; }

        [SerializeField] private SheepLevelConfig _config;

        //It creates a new object pool for CollectionArea objects
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.CollectionArea,
                1,
                false, false,
                transform);
    }
}