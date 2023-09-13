using DG.Tweening;
using AwesomeTools.Sound;
using AwesomeTools;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using UI;

namespace Fishing
{
    public class FishingNet : MonoBehaviour
    {
        public event Action OnCatchEnough;
        public Vector3 NetStorePosition => CalculatePositionInNet();

        [SerializeField] private FishLevelConfig _config;
        [SerializeField] private FishCounter _fishCounter;
        [SerializeField] private Transform _fishStorePosition;
        [SerializeField] private Transform _fishContainer;
        [SerializeField] private BoxCollider2D _netCollider;
        [SerializeField] private float _fishStoreStep;
        [SerializeField] private float _xOffSetMultiplier;
        [SerializeField] private int _fishCountToRandomSpawn;

        private float _fishStoreOffset;
        public int FishCount => _caughtFishes.Count;
        private int _previousFishCount;
        private List<Fish> _caughtFishes = new();
        private Vector3 _cachedScale;
        private Vector3 _spawnPosition;

        private Vector3 Destination { get; set; }

        public SoundSystem SoundSystem { get; private set; }

        /// <summary>
        /// Вводимо точку призначення [destination] та систему звуку [soundSystem]-
        /// запам'ятовуємо розмір та позицію елементу та присвоюємо відповідним полям значення параметрів
        /// </summary>
        public void Construct(Vector3 destination, SoundSystem soundSystem)
        {
            _cachedScale = transform.localScale;
            _spawnPosition = transform.position;
            transform.localScale = Vector3.zero;
            Destination = destination;
            SoundSystem = soundSystem;
            Appear();
        }

        /// <summary>
        /// Переміщає елемент до позиції появи [_spawnPosition]
        /// </summary>
        public void Disappear()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(1);
            sequence.Append(transform.DOMove(_spawnPosition, 0.5f));
        }
        
        /// <summary>
        /// Переміщає елемент до позиції призначення [Destination] та присвоює розмір "_cachedScale"
        /// </summary>
        private void Appear()
        {
            transform.DOMove(Destination, 0.5f);
            transform.DOScale(_cachedScale, 0.1f);
        }

        /// <summary>
        /// Вводимо Collider2D [target] -
        /// перевіряє чи у об'єкту з компонентом "Collider2D" є скрипт "Fish", якщо так, то 
        /// складає рибу у сітку та робить рибу дочірнім елементом та викликає подію "OnCatchEnough",
        /// якщо достатньо риби потрапило у сітку
        /// </summary>
        private void OnTriggerEnter2D(Collider2D target)
        {
            if (TryGetFish(target, out Fish fish))
            {
                if (!fish.IsCatch || fish.IsStored) return;

                fish.Stored();
                _caughtFishes.Add(fish);

                MakeChildOfNet(fish);
                NextStep();

                if (!IsCatchEnough()) return;
                MonitorFishNumbers();
                OnCatchEnough?.Invoke();
            }
        }

        /// <summary>
        /// Вводимо елемент зі скриптом "Fish" [fish] -
        /// присвоює елементу в дочірні елементи параметр "fish" 
        /// </summary>        
        private void MakeChildOfNet(Fish fish)
            => fish.transform.SetParent(_fishContainer);

        /// <summary>
        /// Збільшує значення відступу [_fishStoreOffset] на значення "_fishStoreStep"
        /// </summary>
        private void NextStep()
        {
            _fishStoreOffset += _fishStoreStep;

            if (IsRandomSpawnReady())
                _xOffSetMultiplier *= Random.Range(-1, 1);
            else
                _xOffSetMultiplier *= -1;
        }

        /// <summary>
        /// Повертає значення перевірки: к-сть риби [FishCount] > 
        /// к-сті риби для появи в випадковому місці [_fishCountToRandomSpawn] 
        /// </summary>        
        private bool IsRandomSpawnReady()
            => FishCount > _fishCountToRandomSpawn;

        /// <summary>
        /// Повертає позицію в сітці для риби
        /// </summary>
        private Vector3 CalculatePositionInNet()
        {
            Vector3 offSet = CalculateFishPositionOffSet();
            return _fishStorePosition.position + offSet;
        }
        
        /// <summary>
        /// Повертає значення відступу
        /// </summary>        
        private Vector2 CalculateFishPositionOffSet()
            => new(_fishStoreOffset * _xOffSetMultiplier, _fishStoreOffset);

        /// <summary>
        /// Вмикає можливість взаємодії для пійманих риб [_caughtFishes] та вимикає у сітки [_netCollider]
        /// </summary>
        public void MakeCaughtFishesDraggable()
        {
            _netCollider.enabled = false;

            foreach (Fish caughtFish in _caughtFishes)
                caughtFish.GetComponent<DragAndDrop>().IsDraggable = true;
        }

        // Start "MonitorFishNumbersRoutine" coroutine
        private void MonitorFishNumbers()
        {
            _previousFishCount = GetFishCount();
            StartCoroutine(MonitorFishNumbersRoutine());
        }

        // monitor whether number of fish in "_fishCounter" has reduced, if so, invoke "WithdrawFish()"
        private IEnumerator MonitorFishNumbersRoutine()
        {
            while (_previousFishCount > 0)
            {
                yield return new WaitForSeconds(0.1f);

                int currentFishCount = GetFishCount();
                if (currentFishCount < _previousFishCount)
                {
                    if (_previousFishCount - currentFishCount == 1)
                    {
                        _fishCounter.WithdrawFish();
                    }
                }

                _previousFishCount = currentFishCount;
            }
        }

        // return current number of fish
        private int GetFishCount()
        {
            return _fishContainer.transform.childCount;
        }

        /// <summary>
        /// Вводимо компонент [target] та об'єкт с типом "Fish" [fish] - 
        /// пробує отримати компонент з елементу [target.transform]
        /// </summary>        
        private bool TryGetFish(Component target, out Fish fish)
            => target.transform.TryGetComponent(out fish);

        /// <summary>
        /// Повертає результат перевірки: к-чть риби [FishCount] == к-сті риби в ".config" файлі [_config.FishSpawnCount]
        /// </summary>        
        private bool IsCatchEnough()
            => FishCount == _config.FishSpawnCount;
    }
}