using Carrot.Config;
using Optimization;
using UnityEngine;

namespace Tomato.Spawners
{
    public class WaterPumpPool : MonoBehaviour
    {

        [SerializeField] private TomatoLevelConfig _config;

        public PoolMono<WaterPump> Pool { get; private set; }

        /// <summary>
        /// Створює пул з 1 лійкою
        /// </summary>
        private void Awake()
            => Pool = new PoolMono<WaterPump>(_config.WateringCan, 1, true, true, transform);

    }
}