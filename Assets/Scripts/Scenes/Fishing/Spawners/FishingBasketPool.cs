using Optimization;
using UnityEngine;

namespace Fishing.Spawners
{
    public class FishingBasketPool : MonoBehaviour
    {
        [SerializeField] private FishLevelConfig _config;
        public PoolMono<CollectionArea> Pool { get; private set; }

        /// <summary>
        /// Створює пул з 1 кошиком
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.Basket, 1, false, false, transform);
    }
}