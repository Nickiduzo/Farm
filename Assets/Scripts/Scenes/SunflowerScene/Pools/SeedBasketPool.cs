using Optimization;
using UnityEngine;

namespace SunflowerScene
{
    public class SeedBasketPool : MonoBehaviour
    {
        [SerializeField] private SunflowerConfig _config;
        public PoolMono<CollectionArea> Pool { get; private set; }


        //It creates a new object pool for CollectionArea object
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.Basket, 1, false, false, transform);
    }
}