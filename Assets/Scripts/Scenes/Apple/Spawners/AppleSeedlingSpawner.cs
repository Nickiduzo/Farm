using Apple.Spawners.Pools;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;

namespace Apple
{
    public class AppleSeedlingSpawner : MonoBehaviour
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private SeedlingsApplePool _pool;
        [SerializeField] private Transform _hole;
        [SerializeField] private float _timeToDestruction;

        //Spawns seed
        public SeedlingsApple SpawnSeed()
        {
            SeedlingsApple seedling = _pool.Pool.GetFreeElement();
            seedling.transform.position = _startPoint.position;

            seedling.Construct(_destinationPoint.position, _inputSystem, _soundSystem, _hole.position, _timeToDestruction);
            return seedling;
        }

        // Check free seeds
        public bool HasFreeSeeds()
            => _pool.Pool.HasFreeElement();
    }
}