using Optimization;
using UnityEngine;

namespace Tomato.Spawners.Pools
{
    public class WormsPool : MonoBehaviour
    {
        public PoolMono<Worm> Pool { get; private set; }

        [SerializeField] private TomatoLevelConfig _config;

        /// <summary>
        /// Створює пул з черв'яків к-стю, заданою в ".config" файлі
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<Worm>(_config.Worm,
                _config.WormsToSpawn,
                false, false,
                transform);
    }
}
