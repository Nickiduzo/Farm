using Carrot.Config;
using Optimization;
using UnityEngine;

namespace Carrot.Spawners
{
    public class WaterPumpPool : MonoBehaviour
    {
        public PoolMono<WaterPump> Pool { get; private set; }

        [SerializeField] private CarrotLevelConfig _config;

        // get [WaterPump] from config for pool
        public void Awake()
            => Pool = new PoolMono<WaterPump>(_config.WaterPump, 1, false, false, transform);
    }
}