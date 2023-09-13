using Optimization;
using UnityEngine;

namespace SunflowerScene
{
    public class PestPool : MonoBehaviour
    {
        [SerializeField] private SunflowerConfig _config;
        public PoolMono<Pest> Pool { get; private set; }

        //It creates a new object pool for Pest object
        public void Init(int countInPool){Pool = new PoolMono<Pest>(_config.Pest,
            countInPool,
            false, false,
            transform);}
    }
}