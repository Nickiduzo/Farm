using Optimization;
using UnityEngine;

namespace Tomato.Spawners.Pools
{
    public class WormsBasketPool : MonoBehaviour
    {
        public PoolMono<WormsBasket> Pool { get; private set; }

        [SerializeField] private TomatoLevelConfig _config;

        /// <summary>
        /// Створює пул з 1 банкою для черв'яків
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<WormsBasket>(_config.WormsBasket,
                1,
                false, false,
                transform);
    }
}