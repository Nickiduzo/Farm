using System;
using Sheep.Spawners.Pools;
using AwesomeTools.Sound;
using UnityEngine;

namespace Sheep.Spawners
{
    public class SheepSpawner : MonoBehaviour
    {
        public event Action OnSheepEnded;
        public event Action<Sheep> OnSheepSpawned;

        [SerializeField] private FurStorage _furStorage;
        [SerializeField] private SoundSystem _sound;
        [SerializeField] private FxSystem _fxSystem;
        [SerializeField] private Transform _sheepSpawnPoint;
        [SerializeField] private Transform _sheepStartPoint;
        [SerializeField] private Transform _sheepDestinationPoint;
        [SerializeField] private SheepPool _pool;

        private bool _isWasWhiteSheep;

        // It subscribes to events
        private void Awake()
        {
            _isWasWhiteSheep = !SheepPool.IsMoreWhiteSheep;
        }

        // Spawns a sheep, alternating between white and black, at the specified position
        public void SpawnSheep()
        {
            Sheep sheep;
            if (_isWasWhiteSheep)
            {
                sheep = _pool.BlackSheepPool.GetFreeElement();
                _isWasWhiteSheep = false;
            }
            else
            {
                sheep = _pool.WhiteSheepPool.GetFreeElement();
                _isWasWhiteSheep = true;
            }
            sheep.transform.position = _sheepSpawnPoint.position;

            sheep.OnNextSheep += NextSheep;
            sheep.Construct(_sheepStartPoint.position, _sheepDestinationPoint.position, _furStorage, _sound, _fxSystem);
            OnSheepSpawned?.Invoke(sheep);
        }

        // Handles the logic for spawning the next sheep or invoking the event when no more sheep are available
        private void NextSheep()
        {
            if (!_pool.WhiteSheepPool.HasFreeElement() && !_pool.BlackSheepPool.HasFreeElement())
            {
                OnSheepEnded?.Invoke();
                return;
            }

            SpawnSheep();
        }
    }
}