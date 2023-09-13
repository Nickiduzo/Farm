using Optimization;
using UnityEngine;

namespace Apple
{
    public class VerminPool : MonoBehaviour
    {
        public PoolMono<Vermin> Pool { get; private set; }

        [SerializeField] private AppleLevelConfig _config;

        //It creates a new object pool for Vermin objects
        private void Awake()
            => Pool = new PoolMono<Vermin>(_config.Vermin,
                10,
                false, false,
                transform);
    }
}