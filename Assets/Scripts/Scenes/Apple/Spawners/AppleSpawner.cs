using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System.Collections.Generic;
using UnityEngine;

namespace Apple
{
    public class AppleSpawner : MonoBehaviour
    {
        [SerializeField] private ApplePool _pool;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private AppleHole _hole;
        [SerializeField] private List<Transform> _applePosititions;
        private int index = 0;

        //Spawns apple
        public AppleFruit SpawnApple()
        {
            AppleFruit apple = _pool.Pool.GetFreeElement();
            apple.Construct(_soundSystem, _inputSystem, _hole, _applePosititions[index++]);
            return apple;
        }
    }
}