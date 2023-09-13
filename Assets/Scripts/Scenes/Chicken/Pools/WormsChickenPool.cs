using ChickenScene.Entities;
using Optimization;
using UnityEngine;

namespace ChickenScene.Pools
{
    public class WormsChickenPool : MonoBehaviour
    {
        [SerializeField] private ChickenLevelConfig _config;
        public PoolMono<WormChicken> Pool { get; private set; }
        public int WormsToSpawn => _config.WormsToSpawn;
        
        private void Awake()
            => Pool = new PoolMono<WormChicken>(_config.Worm, _config.WormsToSpawn + 2, false, false, transform);
    }
}