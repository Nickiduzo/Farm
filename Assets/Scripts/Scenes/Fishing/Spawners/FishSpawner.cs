using AwesomeTools.Inputs;
using System;
using AwesomeTools.Sound;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fishing.Spawners
{
    public class FishSpawner : MonoBehaviour
    {
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private FishLevelConfig _config;
        [SerializeField] private float _bounceSpawnOffSetX;
        [SerializeField] private int _extraFishes;

        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _fishesContainerRight;
        [SerializeField] private Transform _fishesContainerLeft;

        public event Action OnFishesSpawned;
        public event Action OnFishesDisappear;
        private Vector3 _lowerRightCorner;
        private Vector3 _upperLeftCorner;
        private int _countToSpawn => _config.FishSpawnCount;

        private readonly int _sortOrderIndexLeftSide = 1;
        private readonly int _sortOrderIndexRightSide = 1;

        /// <summary>
        /// Визначає ліву та праву межу й викликає ф-ції "SpawnFishesLeftSide", "SpawnFishesRightSide"
        /// </summary>
        private void Awake()
        {
            _lowerRightCorner = _camera.ScreenToWorldPoint(new Vector3(0.7f * Screen.width, 0.15f * Screen.height, 1));
            _upperLeftCorner = _camera.ScreenToWorldPoint(new Vector3(1f * Screen.width, 0.45f * Screen.height, 1));

            SpawnFishesLeftSide();
            SpawnFishesRightSide();
        }

        /// <summary>
        /// Викликає подію "OnFishesDisappear"
        /// </summary>
        public void DisappearFishes() => OnFishesDisappear?.Invoke();

        /// <summary>
        /// Створює риби в правому боці та викликає подію "OnFishesSpawned"
        /// </summary>
        private void SpawnFishesRightSide()
        {
            for (int i = 0; i < _countToSpawn + _extraFishes; i++)
            {
                Vector2 range = GetPositionRight();

                Fish fish = Instantiate(_config.Fish, range, _config.Fish.transform.rotation, _fishesContainerRight);
                fish.Construct(_inputSystem, this, _soundSystem);
                fish.ChangeSpriteSortOrder(_sortOrderIndexRightSide);
            }
            OnFishesSpawned?.Invoke();
        }

        /// <summary>
        /// Створює риби в лівому боці та викликає подію "OnFishesSpawned"
        /// </summary>
        private void SpawnFishesLeftSide()
        {
            for (int i = 0; i < _countToSpawn + _extraFishes; i++)
            {
                Vector2 range = GetPositionLeft();

                Fish fish = Instantiate(_config.Fish, range, _config.Fish.transform.rotation, _fishesContainerLeft);
                fish.Construct(_inputSystem, this, _soundSystem);
                fish.ChangeSpriteSortOrder(_sortOrderIndexLeftSide);
            }
            OnFishesSpawned?.Invoke();
        }

        /// <summary>
        /// Повертає позицію в правій частині сцени
        /// </summary>        
        private Vector2 GetPositionRight()
        {
            float spawnX = _camera.ViewportToWorldPoint(new Vector3(1f, Random.Range(0.2f, 0.8f), 0f)).x + GetRandomOffset();
            float spawnY = GetRandomPositionY();
            return new Vector2(spawnX, spawnY);
        }

        /// <summary>
        /// Повертає позицію в лівій частині сцени
        /// </summary>        
        private Vector2 GetPositionLeft()
        {
            float spawnX = _camera.ViewportToWorldPoint(new Vector3(0f, Random.Range(0.2f, 0.8f), 0f)).x - GetRandomOffset();
            spawnX -= 2f;
            float spawnY = GetRandomPositionY();
            return new Vector2(spawnX, spawnY);
        }

        /// <summary>
        /// Повертає випадковий відступ
        /// </summary>        
        private float GetRandomOffset()
        {
            float minOffset = -_bounceSpawnOffSetX;
            float maxOffset = _bounceSpawnOffSetX;
            return Random.Range(minOffset, maxOffset);
        }

        /// <summary>
        /// Повертає випадкове значення    
        /// </summary>        
        private float GetRandomPositionY()
            => Random.Range(_lowerRightCorner.y, _upperLeftCorner.y);
    }
}
