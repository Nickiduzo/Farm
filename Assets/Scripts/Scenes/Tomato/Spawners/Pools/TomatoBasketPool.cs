using Optimization;
using UnityEngine;

namespace Tomato.Spawners.Pools
{
    public class TomatoBasketPool : MonoBehaviour
    {
        public PoolMono<CollectionArea> Pool { get; private set; }

        [SerializeField] private TomatoLevelConfig _config;

        /// <summary>
        /// Створює пул с 1 кошиком для томатів
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.TomatoBasket,
                1,
                false, false,
                transform);
    }
}