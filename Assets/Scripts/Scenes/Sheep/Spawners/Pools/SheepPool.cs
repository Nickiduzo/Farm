using Optimization;
using UnityEngine;

namespace Sheep.Spawners.Pools
{
    public class SheepPool : MonoBehaviour
    {
        public PoolMono<Sheep> WhiteSheepPool { get; private set; }
        public PoolMono<Sheep> BlackSheepPool { get; private set; }

        [SerializeField] private SheepLevelConfig _config;

        public static bool IsMoreWhiteSheep;

        //It creates a new object pool for Sheep objects
        private void Awake()
        {
            int whiteCount = Random.Range(1, 2);
            int blackCount = _config.SheepCount - whiteCount;

            IsMoreWhiteSheep = whiteCount > blackCount;

            WhiteSheepPool = new PoolMono<Sheep>(_config.Sheep,
               whiteCount,
               false, false,
               transform);

            BlackSheepPool = new PoolMono<Sheep>(_config.BlackSheep,
               blackCount,
               false, false,
               transform);
        }
            
    }
}
