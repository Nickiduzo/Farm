using DG.Tweening;
using AwesomeTools.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Sheep
{
    public class FurStorage : MonoBehaviour, IProgressWriter
    {
        private const string FALL_FUR = "FallFur";

        public event Action OnProgressChanged;
        public int CurrentProgress { get; private set; }
        public int MaxProgress => _config.SheepCount;

        [SerializeField] private float _step;
        [SerializeField] private float _movingDuration;
        [SerializeField] private int _storageStackCount;
        [SerializeField] private Transform _storePosition;
        [SerializeField] private SheepLevelConfig _config;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private ActorUI _actorUI;

        private float _startOffSet;
        private int _currentStorageStackCount;
        public List<FurScratch> CaughtFur { get; } = new();

        // It init progress Bar
        private void Awake()
        {
            _actorUI.InitProgressBar(this);
        }

        // Move the fur to the fur storage
        public void MoveToStorage(FurScratch fur)
        {
            fur.transform.SetParent(null);
            ShowTrimFur(fur);
            StartCoroutine(PlayFallSound());
            fur.transform.DOMove(_storePosition.position + CalculateOffSet(), _movingDuration);
            _startOffSet += _step;
            _currentStorageStackCount++;
            CaughtFur.Add(fur);

            if (HasStack())
            {
                IncreaseProgress();
            }
        }

        /// <summary>
        /// Вмикає звук приземлення шерсті трохи раніше перед приземленням
        /// </summary>        
        private IEnumerator PlayFallSound()
        {
            yield return new WaitForSeconds(_movingDuration - .2f);
            _soundSystem.PlaySound(FALL_FUR);
            StopCoroutine(PlayFallSound());
        }
        // Show only the first child object (trimmed fur) and hide the rest
        private static void ShowTrimFur(FurScratch fur)
        {
            var childCount = fur.transform.childCount;
            for (var i = 1; i < childCount; i++)
            {
                fur.transform.GetChild(i).gameObject.SetActive(false);
            }
            fur.transform.GetChild(0).gameObject.SetActive(true);
        }

        // Check if the storage stack is full
        private bool HasStack()
            => _currentStorageStackCount >= _storageStackCount;

        // Increase the progress and invoke the OnProgressChanged event
        private void IncreaseProgress()
        {
            _currentStorageStackCount = 0;
            CurrentProgress++;
            OnProgressChanged?.Invoke();
        }

        // Calculate the offset for the fur's position in the fur storage
        private Vector3 CalculateOffSet()
            => new(Random.Range(-_startOffSet, _startOffSet), 0, 0);
    }
}