using AwesomeTools.Sound;
using AwesomeTools.Inputs;
using System;
using UnityEngine;
using System.Collections;
using UsefulComponents;

namespace Apple
{
    [RequireComponent(typeof(Animator))]
    public class AppleShovel : ShovelBase
    {
        public event Action OnShovelPlaced;

        [SerializeField] private AppleHoleTriggerObserver _observer;

        private BaseHole _currentHole;
        private Animator Animator { get; set; }
        private Vector3 _destinationPoint;
        private Vector3 _spawnPoint;
        private InputSystem _inputSystem;
        private SoundSystem _soundSystem;
        private FxSystem _fxSystem;
        private Vector3 _holePos { get; set; }

        private Coroutine _diggingRoutine;

        private readonly string _stateAnimParam = "State"; 
        private readonly string _digSound = "Dig"; 
        private readonly string _groundFx = "Ground";

        //Constructs Shovel
        public void Construct(Vector3 destinationPoint, Vector3 spawnPoint, InputSystem inputSystem, SoundSystem soundSystem, FxSystem fxSystem, Vector3 holePos)
        {
            _destinationPoint = destinationPoint;
            _spawnPoint = spawnPoint;
            _inputSystem = inputSystem;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
            _holePos = holePos;

            Construct(_soundSystem, _inputSystem, _destinationPoint, _spawnPoint);

            _observer.OnTriggerEnter += StartDigAnimation;
            _observer.OnTriggerExit += StopSound;
            _observer.OnTriggerExit += StopDigAnimation;

            Animator = GetComponent<Animator>();
            MoveToDestinationn();
        }

        // It unsubscribes from events
        protected override void OnDestroy()
        {
            base.OnDestroy();
            _observer.OnTriggerEnter -= StartDigAnimation;
            _observer.OnTriggerExit -= StopSound;
            _observer.OnTriggerExit -= StopDigAnimation;
        }

        // Set "_currentHole"
        private void SetCurrentHole(BaseHole hole)
            => _currentHole = hole; 
        
        // Start the digging animation for the specified hole
        private void StartDigAnimation(BaseHole hole)
        {
            Animator.SetBool(_stateAnimParam, true);
            SetCurrentHole(hole);
        }

        // INVOKE from animation events
        // Start digging coroutine
        public void StartProcessHole()
        {
            _isDigging = true;
            _diggingRoutine ??= StartCoroutine(ProcessHole(_currentHole));
        }

        // Coroutine for the digging process
        private IEnumerator ProcessHole(BaseHole hole)
        {
            if(_currentHole != null){
                while (_isDigging)
                {                       
                    hole.AddProgress(Time.deltaTime * 7);
                    Debug.Log("AddProgress");
                    yield return new WaitForSeconds(0.05f);                
                }
            }            
        }

        // Stop the digging animation for the specified hole, stop "ProcessHole" coroutine
        private void StopDigAnimation(BaseHole hole)
        {
            _currentHole = null;
            Animator.SetBool(_stateAnimParam, false);
            _isDigging = false;

            if(_diggingRoutine != null)
                StopCoroutine(_diggingRoutine);

            _diggingRoutine = null;
        }

        // Move to the destination and invoke the shovel placed event
        public void MoveToDestinationn()
        {
            MoveToDestination();
            OnShovelPlaced?.Invoke();
            ActivateHint();
        }

        // Activate the hint pointer
        private void ActivateHint()
            => HintSystem.Instance.ShowPointerHint(_move.Destination, _holePos);

        // Stop the sound for the specified hole
        private void StopSound(BaseHole hole)
            => _soundSystem.StopSound(_digSound);

        // INVOKE from animation events
        // Play "_digSound" sound
        public void PlaySound()
            => _soundSystem.PlaySound(_digSound);

        // INVOKE from animation events
        // Appear ground FX 
        public void PlayGroundFX()
            => _fxSystem.PlayEffect(_groundFx, _holePos);

        // End the life cycle of the shovel
        public void EndLifeCycle()
        {
            MakeNonInteractable();
            MoveToStartPoint();
        }
    }
}