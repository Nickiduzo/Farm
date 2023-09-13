using System.Collections.Generic;
using System.Linq;
using AwesomeTools.Inputs;
using UnityEngine;

namespace SunflowerScene
{
    public class SeedSpawner : MonoBehaviour
    {
        [SerializeField] private SeedPool _seedPool;
        [SerializeField] private Vector3 _offset;
        [SerializeField] private Transform _destination;
        private readonly List<Seed> _seeds = new();
        private InputSystem _inputSystem;
        private SunflowerSeedPosInBasket _seedPosInBasket;
        //Init input system
        public void Constructor(InputSystem inputSystem)
        {
            _inputSystem = inputSystem;
        }

        // Spawns a seed at the specified position, constructs it, enables it, and adds it to the list of seeds.
        public void Spawn(Vector3 position)
        {
            var seed = _seedPool.Pool.GetFreeElement();
            seed.transform.position = position;
            _seedPosInBasket = GetComponent<SunflowerSeedPosInBasket>();
            seed.Construct(position + _offset, _inputSystem,_seedPosInBasket);
            seed.Enable(position, _offset);
            _seeds.Add(seed);
        }
        // Returns the position of any seed in the list of seeds.
        public Vector3 GetEnySeedPosition()
        {
            return _seeds.First().transform.position;
        }
    }
}