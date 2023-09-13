using Optimization;
using UnityEngine;

namespace SunflowerScene
{
    public class SeedPool : MonoBehaviour
    {
        [SerializeField] private SunflowerConfig _config;
        [SerializeField] private int _countInPool = 4;
        public PoolMono<Seed> Pool { get; private set; }

        //It creates a new object pool for Seed object
        public void Awake() =>
            Pool = new PoolMono<Seed>(_config.Seed,
                _countInPool,
                false, true,
                transform);
    }
}