using Carrot.Config;
using Optimization;
using UnityEngine;

namespace Carrot.Spawners
{
    public class CarrotBasketPool : MonoBehaviour
    {
        [SerializeField] private CarrotLevelConfig _config;
        public PoolMono<CollectionArea> Pool { get; private set; }

        // get [CollectionArea] from config for pool
        private void Awake()
            => Pool = new PoolMono<CollectionArea>(_config.CollectionArea, 1, false, false, transform);
    }
}