using Optimization;
using UnityEngine;

namespace Apple
{
    public class AppleWaterPumpPool : MonoBehaviour
    {
        public PoolMono<AppleWaterPump> Pool { get; private set; }

        [SerializeField] private AppleLevelConfig _config;

        //It creates a new object pool for AppleWaterPump objects
        private void Awake()
            => Pool = new PoolMono<AppleWaterPump>(_config.WaterPump,
                1,
                false, false,
                transform);
    }
}