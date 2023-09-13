using DG.Tweening;
using AwesomeTools.Inputs;
using System;
using System.Collections;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Carrot
{
    public class SeedPackage : MonoBehaviour
    {
        public event Action<Vector3> OnPutInHole;
        public event Action OnSeedBuried;

        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private float _seedSpawnRate;
        [SerializeField] private CarrotHoleTriggerObserver _observer;
        [SerializeField] private DragAndDrop _dragAndDrop;
        private Coroutine _seedRoutine;
        private CarrotHole _carrotHole;
        private bool _isSeeding;
        private Vector3 _spawnPoint;
        private Vector3 _destination;

        // subscribe Actions [OnTriggerEnter], [OnTriggerExit] and set up [MoveToDestinationOnDragEnd],
        // [DragAndDrop], [_spawnPoint] and [_destination]
        public void Construct(Vector3 destination, Vector3 spawnPoint, InputSystem input)
        {
            _spawnPoint = spawnPoint;
            _destination = destination;
            _destinationOnDragEnd.Construct(destination);
            _destinationOnDragEnd.MoveToDestination();
            _dragAndDrop.Construct(input);

            _observer.OnTriggerEnter += StartSeedRoutine;
            _observer.OnTriggerExit += _ => StopSeedRoutine();
        }

        private void Start()
            => ActivateHint(_destination);
        
        private void OnDestroy()
        {
            _observer.OnTriggerEnter -= StartSeedRoutine;
            _observer.OnTriggerExit -= _ => StopSeedRoutine();
        }

        // stop seeds dropping from package (invoke when hole is completely filled or when seed package leave trigger area near hole)  
        private void StopSeedRoutine()
        {
            _isSeeding = false;
            if (_seedRoutine != null)
                StopCoroutine(_seedRoutine);
        }

        // check if all holes is completely filled, if so, return see
        private void TryBurry()
        {
            if (!IsFullOfSeeds(_carrotHole)) return;
            BuryHole();
        }

        // invoke when seed package in tregger area near hole and launch [SeedRoutine]
        private void StartSeedRoutine(CarrotHole carrotHole)
        {
            if (_isSeeding) return;

            _isSeeding = true;
            _seedRoutine = StartCoroutine(SeedRoutine(carrotHole));
        }

        // check if hole recived 3 seeds, if so, create dirt on it
        private bool IsFullOfSeeds(CarrotHole carrotHole)
            => carrotHole.SeedsCount >= 3;

        // start pouring seeds out of package
        private IEnumerator SeedRoutine(CarrotHole carrotHole)
        {
            _carrotHole = carrotHole;

            while (_isSeeding)
            {
                yield return new WaitForSeconds(_seedSpawnRate);
                OnPutInHole?.Invoke(transform.position);
            }
        }

        // hide hint invoke "ContactArea" which moves seed to hole
        public void PutInHole(Seed seed)
        {
            DisableHint();
            _carrotHole.AddSeed(seed);
            ContactArea.Instance.MoveSeedToHole(_carrotHole.SeedDestination, seed.transform, 0.3f)
                .OnComplete(() =>TryBurry());
        }

        // set position for hint and enable it
        private void ActivateHint(Vector3 startPosition)
            => HintSystem.Instance.ShowPointerHint(startPosition, Vector3.zero);
        
        private void DisableHint()
            => HintSystem.Instance.HidePointerHint();
        
        // invoke Action [OnSeedBuried] and start apper dirt in hole, stop pouring seeds out of package
        private void BuryHole()
        {
            _carrotHole.Bury();
            OnSeedBuried?.Invoke();
            StopSeedRoutine();
        }

        // seed package leave scene
        private void MoveToStart()
            => transform.DOMove(_spawnPoint, 0.5f);

        // make unavaible for dragging 
        public void EndLifeCycle()
        {
            _destinationOnDragEnd.MovingTween.Kill();
            _dragAndDrop.IsDraggable = false;
            MoveToStart();
        }
    }
}