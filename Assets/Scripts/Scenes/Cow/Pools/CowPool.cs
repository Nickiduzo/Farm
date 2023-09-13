using Optimization;
using UnityEngine;

namespace CowScene.Spawners.Pools
{
    public class CowPool : MonoBehaviour
    {
        public PoolMono<Cow> Pool { get; private set; }

        [SerializeField] private CowLevelConfig _config;

        //It creates a new object pool for Cow objects
        private void Awake()
            => Pool = new PoolMono<Cow>(_config.Cow,
                1,
                false, false,
                transform);
    }
}

