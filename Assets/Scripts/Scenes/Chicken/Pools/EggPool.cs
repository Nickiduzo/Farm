using ChickenScene.Entities;
using Optimization;
using UnityEngine;

namespace ChickenScene.Pools
{
    public class EggPool : MonoBehaviour
    {
        [SerializeField] private ChickenLevelConfig _config;
        public PoolMono<Egg> Pool { get; private set; }
        public int EggToWin => _config.EggToWin;
        
        private void Awake()
            => Pool = new PoolMono<Egg>(_config.Egg,
                10,
                false, true,
                transform);
    }
}