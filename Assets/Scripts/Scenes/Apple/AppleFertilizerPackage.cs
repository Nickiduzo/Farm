using AppleTree;
using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AwesomeTools;
using UsefulComponents;

namespace Apple
{
    public class AppleFertilizerPackage : MonoBehaviour, IHintable
    {
        private const string FERTILIZER = "Fertilizer";

        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private float _fertilizerHeapSpawnRate;
        [SerializeField] private AppleFertilizerContainerTriggerObserver _observer;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private Transform _fxPoint;

        [SerializeField] private float _rotateDuration;
        [SerializeField] private float _rotateZAngle;
        private Quaternion _cachedRotation;

        private List<AppleFertilizerPlace> _fertilizerPlaces;
        private Coroutine _fertilizerRoutine;
        private bool _isFertilizing;

        private Vector3 _spawnPoint;
        private Vector3 _startPoint;
        private Vector3 HolePos { get; set; }

        private SoundSystem _soundSystem;
        private FxSystem _fxSystem;

        //Init AppleFertilizerPackage and subscribe to the events
        public void Construct(Vector3 destination, Vector3 spawnPoint, InputSystem input, AppleHole hole, SoundSystem soundSystem, FxSystem fxSystem)
        {
            _spawnPoint = spawnPoint;
            _startPoint = destination;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
            HolePos = hole.transform.position;
            _cachedRotation = transform.rotation;
            _destinationOnDragEnd.Construct(destination);
            _destinationOnDragEnd.MoveToDestination();
            _dragAndDrop.Construct(input);

            _dragAndDrop.OnDragStart += DeactivateHint;

            _observer.OnTriggerEnter += StartFertilizingProcess;
            _observer.OnTriggerExit += _ => StopFertilizingProcess();

            MoveToStart();
        }

        // It unsubscribes from events
        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= DeactivateHint;

            _observer.OnTriggerEnter -= StartFertilizingProcess;
            _observer.OnTriggerExit -= _ => StopFertilizingProcess();
        }


        // End the life cycle of the apple fertilizer
        public void EndLifeCycle()
        {
            _destinationOnDragEnd.MovingTween.Kill();
            _dragAndDrop.IsDraggable = false;
            MoveToSpawnPoint();
        }

        // Activate the hint for the apple fertilizer
        public void ActivateHint()
        {
            HintSystem.Instance.ShowPointerHint(_startPoint, HolePos);
        }

        // Deactivate the hint for the apple fertilizer
        public void DeactivateHint()
        {
            HintSystem.Instance.HidePointerHint();
        }

        // Start the process of fertilizing apples
        private void StartFertilizingProcess(AppleFertilizerContainer fertilizerContainer)
        {
            RotateOnStartFertilizing();
            StartSound();
            StartFX();
            StartFertilizeRoutine(fertilizerContainer);
        }

        // Stop the process of fertilizing apples
        private void StopFertilizingProcess()
        {
            RotateToNormalState();
            StopSound();
            StopFX();
            StopFertilizeRoutine();
        }

        // Start the coroutine for fertilizing apples
        private void StartFertilizeRoutine(AppleFertilizerContainer fertilizerContainer)
        {
            if (_fertilizerPlaces == null)
            {
                _fertilizerPlaces = fertilizerContainer.HolesOnScene;
            }

            if (_isFertilizing)
            {
                return;
            }

            _isFertilizing = true;
            _fertilizerRoutine = StartCoroutine(FertilizeRoutine());
        }

        // Coroutine for fertilizing apples
        private IEnumerator FertilizeRoutine()
        {
            foreach (var hole in _fertilizerPlaces)
            {
                if (!hole.IsFertilized)
                {
                    yield return new WaitForSeconds(_fertilizerHeapSpawnRate);
                    PutOnPlace(hole);
                }
            }
        }

        // Stop the routine for fertilizing apples
        private void StopFertilizeRoutine()
        {
            foreach (var hole in _fertilizerPlaces)
            {
                if (_fertilizerRoutine != null)
                    StopCoroutine(_fertilizerRoutine);

                _isFertilizing = false;
            }
        }

        // Move the apple fertilizer to the start point
        private void MoveToStart()
        {
            transform.DOMove(_startPoint, 0.5f);
            ActivateHint();
        }

        // Start the visual effect for the apple fertilizer
        private void StartFX() => _fxSystem.PlayEffect(FERTILIZER, _fxPoint.position, transform);

        // Stop the visual effect for the apple fertilizer
        private void StopFX() => _fxSystem.StopEffect(FERTILIZER);

        // Start the sound effect for the apple fertilizer
        private void StartSound() => _soundSystem.PlaySound(FERTILIZER);

        // Stop the sound effect for the apple fertilizer
        private void StopSound() => _soundSystem.StopSound(FERTILIZER);

        // Put the apple fertilizer on a specific place
        private void PutOnPlace(AppleFertilizerPlace hole) => hole.StoreSeed();

        // Move the apple fertilizer to the spawn point
        private void MoveToSpawnPoint() => transform.DOMove(_spawnPoint, 0.5f);

        // Rotate the apple fertilizer when starting the fertilizing process
        private void RotateOnStartFertilizing()
            => transform.DORotate(new Vector3(0, 0, _rotateZAngle), _rotateDuration);

        // Rotate the apple fertilizer to its normal state
        private void RotateToNormalState()
            => transform.DORotateQuaternion(_cachedRotation, _rotateDuration);

    }
}