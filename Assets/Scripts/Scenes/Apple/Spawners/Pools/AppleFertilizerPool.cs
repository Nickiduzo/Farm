using Optimization;
using UnityEngine;

namespace Apple
{
    public class AppleFertilizerPool : MonoBehaviour
    {
        public PoolMono<AppleFertilizerPackage> Pool { get; private set; }

        [SerializeField] private AppleLevelConfig _config;

        //It creates a new object pool for AppleFertilizerPackage objects
        private void Awake()
            => Pool = new PoolMono<AppleFertilizerPackage>(_config.FertilizerPackage,
                1,
                false, false,
                transform);
    }
}