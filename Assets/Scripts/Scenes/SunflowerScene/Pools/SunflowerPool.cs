using Optimization;
using UnityEngine;

namespace SunflowerScene
{
    public class SunflowerPool : MonoBehaviour
    {
        [SerializeField] private SunflowerConfig _config;
        [SerializeField] private int _countInPool = 4;

        public PoolMono<Sunflower> Pool { get; private set; }

        //It creates a new object pool for Sunflower object
        public void Awake() =>
            Pool = new PoolMono<Sunflower>(_config.Sunflower,
                _countInPool,
                true, false,
                transform);
    }
}