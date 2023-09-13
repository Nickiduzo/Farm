using Optimization;
using UnityEngine;

namespace Sheep.Spawners.Pools
{
    public class TrimmerPool : MonoBehaviour
    {
        public PoolMono<Trimmer> Pool { get; private set; }

        [SerializeField] private SheepLevelConfig _config;

        //It creates a new object pool for Trimmer objects
        private void Awake()
            => Pool = new PoolMono<Trimmer>(_config.Trimmer,
                1,
                false, false,
                transform);
    }
}