using Bee.Config;
using Optimization;
using UnityEngine;

namespace Bee.Spawners
{
    public class RecyclerPool : MonoBehaviour
    {
        [SerializeField] private BeeLevelConfig _config;
        public PoolMono<HoneyRecycler> Pool { get; private set; }

        //It creates a new object pool for HoneyRecycler objects
        private void Awake()
            => Pool = new PoolMono<HoneyRecycler>(_config.HoneyRecycler, 1, false, false, transform);
    }
}