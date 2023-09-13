using Optimization;
using UnityEngine;

namespace SunflowerScene
{
    public class WaterPumpPool : MonoBehaviour
    {
        [SerializeField] private SunflowerConfig _config;
        public PoolMono<WaterPump> Pool { get; private set; }

        //It creates a new object pool for WaterPump object
        private void Awake()
            => Pool = new PoolMono<WaterPump>(_config.WaterPump, 1, false, false, transform);
    }
}