using Optimization;
using UnityEngine;

namespace Sheep.Spawners.Pools
{
    public class ComposedFurPool : MonoBehaviour
    {
        public PoolMono<ComposedFur> Pool { get; private set; }

        [SerializeField] private SheepLevelConfig _config;

        //It creates a new object pool for ComposedFur objects
        private void Awake()
            => Pool = new PoolMono<ComposedFur>(_config.ComposedFur,
                _config.ComposedFurToSpawn,
                false, false,
                transform);
    }
}