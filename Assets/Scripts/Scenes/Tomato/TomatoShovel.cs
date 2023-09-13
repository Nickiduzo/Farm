using DG.Tweening;
using AwesomeTools.Sound;
using System;
using AwesomeTools.Inputs;
using UnityEngine;
using System.Collections;

namespace Tomato
{
    [RequireComponent(typeof(Animator))]
    public class TomatoShovel : ShovelBase
    {
        private const string DIG = "Dig";

        [SerializeField] private Transform _fxSpawnPoint;
        [SerializeField] private float _diggingCoolDown;
        [SerializeField] private TomatoHoleTriggerObserver _observer;

        private TomatoHole _currentHole;
        private Coroutine _diggingRoutine;
        private bool _isHaveOnAnimateAction;

        private Vector3 _destinationPoint;
        private Vector3 _spawnPoint;
        private InputSystem _inputSystem;
        private SoundSystem _soundSystem;

        public event Action OnLifeCycleEnded;
        public event Action OnAnimationEvent;

        // Initializes the instance with provided parameters and sets up event listeners
        public void Construct(Vector3 destinationPoint, Vector3 spawnPoint, InputSystem inputSystem, SoundSystem soundSystem)
        {
            _destinationPoint = destinationPoint;
            _spawnPoint = spawnPoint;
            _inputSystem = inputSystem;
            _soundSystem = soundSystem;

            Construct(_soundSystem, _inputSystem, _destinationPoint, _spawnPoint);

            _observer.OnTriggerStay += ProcessHole;
            _observer.OnTriggerExit += StopDigging;

            MoveToDestination();
        }

        // Removes event listeners when the object is destroyed
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _observer.OnTriggerStay -= ProcessHole;
            _observer.OnTriggerExit -= StopDigging;
        }

        // USING in animation events
        // Handles the completion of an animation by invoking associated events and playing a sound
        public void AnimationCompleteHandler()
            => OnAnimationEvent?.Invoke();

        // USING in animation events
        // Play [DIG] sound
        public void PlayDigSound()
            => _soundSystem.PlaySound(DIG);

        // Sets up animation completion events for a specific TomatoHole
        private void SetHoleAnimationComplete(TomatoHole hole)
        {
            if (hole != null && !_isHaveOnAnimateAction)
            {
                OnAnimationEvent += () => hole.ChangeSprite();
                OnAnimationEvent += () => ShowFx(_fxSpawnPoint);
                _isHaveOnAnimateAction = true;
            }
        }

        // Processes the interaction with a TomatoHole
        private void ProcessHole(TomatoHole hole)
        {
            if (_currentHole != null && _currentHole != hole)
            {
                StopDigging(hole);
            }
            else if (_currentHole == null || _currentHole == hole)
            {
                _currentHole = hole;
                SetHoleAnimationComplete(hole);
                hole.AddProgress(Time.deltaTime * 5);
                DeactivateHint();

                if (_diggingRoutine != null)
                {
                    return;
                }

                _isDigging = true;
                _diggingRoutine = StartCoroutine(DiggingRoutine());
            }
        }

        // Stops the digging process, resets animation state, and performs additional actions on a specified TomatoHole
        public void StopDigging(TomatoHole hole = null)
        {
            _isDigging = false;

            if (IsAnimatorAvailable())
            {
                SetDig(false);
            }

            if (_diggingRoutine == null)
            {
                return;
            }

            StopCoroutine(_diggingRoutine);
            _diggingRoutine = null;

            if (hole != null)
            {
                if (!hole.IsThisLastSprite())
                {
                    _currentHole = null;
                }
                else if (hole.IsThisLastSprite())
                {
                    if (hole.IsDig == false)
                    {
                        hole.Dig();
                    }

                    _currentHole = null;
                }

                OnAnimationEvent = null;
                _isHaveOnAnimateAction = false;
            }
        }

        // Coroutine for the digging process
        private IEnumerator DiggingRoutine()
        {
            while (_isDigging)
            {
                if (IsAnimatorAvailable())
                {
                    SetDig(true);
                }

                yield return new WaitForSeconds(_diggingCoolDown);
                StopDigging();
            }
        }

        // Retrieves the destination vector
        public Vector3 GetDestination()
        {
            return _move.Destination;
        }

        //USING in animation events
        // Clears the current TomatoHole if it exists and meets certain conditions
        private void ClearCurrentHole()
        {
            if (_currentHole == null)
            {
                return;
            }

            if (_currentHole.IsThisLastSprite())
            {
                StopDigging(_currentHole);
            }
        }

        // Ends the lifecycle of the object by making it non-interactable and moving it to the start point
        public void EndLifeCycle()
        {
            MakeNonInteractable();
            MoveToStartPoint().OnComplete(() => OnLifeCycleEnded?.Invoke());
        }

    }
}