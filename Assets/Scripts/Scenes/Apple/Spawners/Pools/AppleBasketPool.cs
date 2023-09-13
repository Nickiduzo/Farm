using Optimization;
using UnityEngine;

namespace Apple
{
    public class AppleBasketPool : MonoBehaviour
    {
        public PoolMono<CollectionArea> Pool { get; private set; }

        [SerializeField] private AppleLevelConfig _config;

        //It creates a new object pool for CollectionArea objects
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.Basket,
                1,
                false, false,
                transform);
    }
}