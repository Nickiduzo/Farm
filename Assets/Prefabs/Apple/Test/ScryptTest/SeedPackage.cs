using DG.Tweening;
using AwesomeTools.Inputs;
using System;
using System.Collections;
using UnityEngine;
using AwesomeTools;

namespace Apple
{
    public class SeedPackage : MonoBehaviour
    {
        public event Action<Vector3> OnPutInHole;
        public event Action OnSeedBuried;

        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private float _seedSpawnRate;
        [SerializeField] private HoleTriggerObserver _observer;
        [SerializeField] private DragAndDrop _dragAndDrop;
        private Coroutine _seedRoutine;
        private Hole _hole;
        private bool _isSeeding;
        private Vector3 _spawnPoint;

        public void Construct(Vector3 destination, Vector3 spawnPoint, InputSystem input)
        {
            _spawnPoint = spawnPoint;

            _destinationOnDragEnd.Construct(destination);
            _destinationOnDragEnd.MoveToDestination();

            _dragAndDrop.Construct(input);
            _dragAndDrop.OnDragStart += RotateToFeed;
            _dragAndDrop.OnDragEnded += RotateToStart;
            _observer.OnTriggerEnter += StartSeedRoutine;
            _observer.OnTriggerExit += _ => StopSeedRoutine();
        }

        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= RotateToFeed;
            _dragAndDrop.OnDragEnded -= RotateToStart;
            _observer.OnTriggerEnter -= StartSeedRoutine;
            _observer.OnTriggerExit -= _ => StopSeedRoutine();
        }

        private void StopSeedRoutine()
        {
            if (_seedRoutine != null)
                StopCoroutine(_seedRoutine);

            _isSeeding = false;
        }

        private void StartSeedRoutine(Hole hole)
        {
            if (_isSeeding) return;

            _isSeeding = true;
            _seedRoutine = StartCoroutine(SeedRoutine(hole));
        }

        private bool IsFullOfSeeds(Hole hole)
            => hole.SeedsCount >= 3;

        private IEnumerator SeedRoutine(Hole hole)
        {
            _hole = hole;

            while (_isSeeding)
            {
                yield return new WaitForSeconds(_seedSpawnRate);

                if (IsFullOfSeeds(hole))
                {
                    hole.Bury();
                    OnSeedBuried?.Invoke();
                    StopSeedRoutine();
                }
                else
                    OnPutInHole?.Invoke(transform.position);
            }
        }

        public void PutInHole(Seed seed)
        {
            seed.MoveTo(_hole.SeedDestination)
                .OnComplete(() => _hole.AddSeed(seed));
        }


        private void MoveToStart()
            => transform.DOMove(_spawnPoint, 0.5f);

        private void RotateToFeed()
            => transform.DORotate(new Vector3(0, 0, 120), 0.5f);

        private void RotateToStart()
            => transform.DORotate(new Vector3(0, 0, 0), 0.5f);

        public void EndLifeCycle()
        {
            _destinationOnDragEnd.MovingTween.Kill();
            _dragAndDrop.IsDraggable = false;
            MoveToStart();
        }
    }
}