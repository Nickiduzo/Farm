using Optimization;
using UnityEngine;


namespace CowScene.Spawners.Pools
{
    public class MilkJarPool : MonoBehaviour
    {
        public PoolMono<Jar> Pool { get; private set; }

        [SerializeField] private CowLevelConfig _config;

        //It creates a new object pool for Jar objects
        private void Awake()
            => Pool = new PoolMono<Jar>(_config.Jar,
                _config.JarCount,
                false, false,
                transform);
    }


}
