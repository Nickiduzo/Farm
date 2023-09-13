using Optimization;
using UnityEngine;

namespace Tomato.Spawners.Pools
{
    public class TomatoShovelPool : MonoBehaviour
    {
        public PoolMono<TomatoShovel> Pool { get; private set; }

        [SerializeField] private TomatoLevelConfig _config;

        /// <summary>
        /// Створює пул з 1 лопатою
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<TomatoShovel>(_config.Shovel, 1, false, false, transform);
    }
}