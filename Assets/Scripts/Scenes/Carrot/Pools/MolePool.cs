using Carrot.Config;
using Optimization;
using UnityEngine;

namespace Carrot.Spawners
{
    public class MolePool : MonoBehaviour
    {
        [SerializeField] private CarrotLevelConfig _config;
        public PoolMono<Mole> Pool { get; private set; }

        // get [Mole] from config fro pool, get number of moles that will appear from config 
        private void Awake()
            => Pool = new PoolMono<Mole>(_config.Mole, _config.MaxMoleToSpawn + 1, true, true, transform);
    }
}