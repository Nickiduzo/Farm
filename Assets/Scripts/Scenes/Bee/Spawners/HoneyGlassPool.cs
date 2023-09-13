using Bee.Config;
using Optimization;
using UnityEngine;

namespace Bee.Spawners
{
    public class HoneyGlassPool : MonoBehaviour
    {

        [SerializeField] private BeeLevelConfig _config;
        public PoolMono<HoneyGlass> Pool { get; private set; }

        //It creates a new object pool for HoneyGlass objects
        private void Awake()
            => Pool = new PoolMono<HoneyGlass>(_config.HoneyGlass, _config.GlassToSpawn, false, false, transform);
    }
}