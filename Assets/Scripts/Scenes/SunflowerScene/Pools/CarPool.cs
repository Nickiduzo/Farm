using Optimization;
using UnityEngine;

namespace SunflowerScene
{
    public class CarPool : MonoBehaviour
    {
        [SerializeField] private SunflowerConfig _config;
        public PoolMono<Car> Pool { get; private set; }

        //It creates a new object pool for Car object
        private void Awake()
            => Pool = new PoolMono<Car>(_config.Car, 1, false, false, transform);
    }
}