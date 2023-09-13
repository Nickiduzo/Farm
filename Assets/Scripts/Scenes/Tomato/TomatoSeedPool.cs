using Optimization;
using UnityEngine;

namespace Tomato.Spawners.Pools
{
    public class TomatoSeedPool : MonoBehaviour
    {
        public PoolMono<TomatoSeed> Pool { get; private set; }

        [SerializeField] private TomatoLevelConfig _config;

        /// <summary>
        /// Створює пул з 2 саженцями томатів
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<TomatoSeed>(_config.TomatoSeed,
                2,
                false, false,
                transform);
    }
}