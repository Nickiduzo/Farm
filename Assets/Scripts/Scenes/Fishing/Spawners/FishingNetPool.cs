using Optimization;
using UnityEngine;

namespace Fishing.Spawners
{
    public class FishingNetPool : MonoBehaviour
    {
        [SerializeField] private FishLevelConfig _config;
        public PoolMono<FishingNet> Pool { get; private set; }

        /// <summary>
        /// Створює пул с 1 сіткою
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<FishingNet>(_config.Net, 1, false, false, transform);
    }
}