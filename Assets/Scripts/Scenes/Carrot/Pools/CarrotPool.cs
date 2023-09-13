using Carrot.Config;
using Optimization;
using UnityEngine;

namespace Carrot.Spawners
{
    public class CarrotPool : MonoBehaviour
    {
        [SerializeField] private CarrotLevelConfig _config;
        public PoolMono<Carrot> Pool { get; private set; }

        // get [Carrot] from config for pool, set number of carrots that will appear
        private void Awake()
            => Pool = new PoolMono<Carrot>(_config.Carrot, 6, true, true, transform);
    }
}