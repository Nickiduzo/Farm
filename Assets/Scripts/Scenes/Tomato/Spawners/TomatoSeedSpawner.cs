using System.Collections.Generic;
using AwesomeTools.Inputs;
using Tomato.Spawners.Pools;
using UnityEngine;
using AwesomeTools.Sound;

namespace Tomato
{
    public class TomatoSeedSpawner : MonoBehaviour
    {
        private const float X_POS = 0.85f;
        private const float Y_POS = 0.13f;
        private Vector3 _destinationPoint;
        [SerializeField] private Camera _camera;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private TomatoSeedPool _pool;                
        private List<TomatoSeed> _spawnedTomatoSeeds = new();

        /// <summary>
        /// Викликає саженець томатів
        /// </summary>
        public TomatoSeed SpawnSeed()
        {
            TomatoSeed seed = _pool.Pool.GetFreeElement();
            seed.transform.position = _startPoint.position;
            CalculateDestinationPoint();
            
            seed.Construct(_destinationPoint ,_inputSystem, _soundSystem);
            _spawnedTomatoSeeds.Add(seed);
            return seed;
        }

        /// <summary>
        /// Перевіряє чи є ще саженці
        /// </summary>        
        public bool HasFreeSeeds() 
            => _pool.Pool.HasFreeElement();

        public void SetReservDestinationPointToSpawnedSeed()
        {
            foreach (TomatoSeed seed in _spawnedTomatoSeeds)
            {
                seed.SetReservDestinationPointToSpawnedTomato();
            }
        }

        /// <summary>
        /// Рахує позицію призначення [_destinationPoint]
        /// </summary>
        private void CalculateDestinationPoint()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _destinationPoint = new Vector3(destination.x, destination.y, destination.z);
        }
        
        /// <summary>
        /// Повертає позицію призначення [_destinationPoint]
        /// </summary>        
        public Vector3 GetDestinationPoint() 
            => _destinationPoint;
    }
}