using Bee.Config;
using Bee.Triggers;
using DG.Tweening;
using AwesomeTools.Sound;
using System;
using System.Collections;
using UI;
using UnityEngine;
using AwesomeTools;
using UsefulComponents;

namespace Bee
{
    public class Sprayer : MonoBehaviour, IProgressWriter
    {
        private const string SPRAY = "Spray";
        private const string SUCCESS = "Success";
        public event Action OnProgressChanged;
        public event Action OnAllBeesCatch;

        [SerializeField] private MoveStartDestination _move;
        [SerializeField] private BeeLevelConfig _config;
        [SerializeField] private ParticleSystem _sprayFx;
        [SerializeField] private Transform _sprayPoint;
        [SerializeField] private Transform _sprayWing;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private Collider2D _sprayAreaCollider;
        [SerializeField] private BeeTriggerObserver _trigger;

        private bool _isSpraying;
        private bool _isSequenceActive;
        private Sequence _movementSequence;
        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;
        private Vector3 _startPosSprayWing;
        public int CurrentProgress { get; private set; }
        public int MaxProgress => _config.BeeToSpawn;

        //Its init some systems
        public void Construct(ISoundSystem soundSystem, FxSystem fxSystem)
        {
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
        }

        // It subscribes from events

        private void Awake()
        {
            _startPosSprayWing = _sprayWing.localPosition;
            CreateMovementSequence();
            StopMovementSprayWing();

            _trigger.OnTriggerEnter += ProcessBee;
            _dragAndDrop.OnDragStart += EnableSpraying;
            _dragAndDrop.OnDragEnded += DisableSpraying;
        }
        // It unsubscribes from events
        private void OnDestroy()
        {
            _trigger.OnTriggerEnter -= ProcessBee;
            _dragAndDrop.OnDragStart -= EnableSpraying;
            _dragAndDrop.OnDragEnded -= DisableSpraying;
        }

        // Process the caught bee
        private void ProcessBee(Bee bee)
        {
            if (CurrentProgress == 0)
            {
                HideHint();
            }

            bee.Catch();
            _soundSystem.PlaySound(SUCCESS);
            _fxSystem.PlayEffect(SUCCESS, bee.transform.position);
            CurrentProgress++;
            OnProgressChanged?.Invoke();

            if (CurrentProgress < MaxProgress)
            {
                return;
            }

            MakeNonInteractable();
            _move.MoveToStart();
            OnAllBeesCatch?.Invoke();
        }

        // Make the game object non-interactable
        private void MakeNonInteractable()
        {
            DOTween.Kill(gameObject);
            _dragAndDrop.IsDraggable = false;
            _sprayAreaCollider.enabled = false;
            DisableSpraying();
        }

        // Enable bee spraying
        private void EnableSpraying()
        {
            StartMovementSprayWing();
            _sprayAreaCollider.enabled = true;
            _isSpraying = true;
            StartCoroutine(SprayingRoutine());
        }

        // Disable bee spraying
        private void DisableSpraying()
        {
            StopMovementSprayWing();
            _soundSystem.StopSound(SPRAY);
            _sprayAreaCollider.enabled = false;
            _isSpraying = false;
        }

        // Provides up and down movement for "SprayWing"
        private void CreateMovementSequence()
        {
            _movementSequence = DOTween.Sequence()
                .SetAutoKill(false)
                .Append(_sprayWing.DOLocalMoveY(_startPosSprayWing.y + 0.6f, 0.3f))
                .Append(_sprayWing.DOLocalMoveY(_startPosSprayWing.y, 0.3f))
                .SetLoops(-1)
                .Pause();
        }

        // Start moving "SprayWing"
        public void StartMovementSprayWing()
        {
            if (!_isSequenceActive)
                _isSequenceActive = true;
                _movementSequence.Play();
        }

        // Stop moving "SprayWing"
        public void StopMovementSprayWing()
        {
            if (_isSequenceActive)
                _isSequenceActive = false;
                _movementSequence.Pause();
        }

        // Coroutine for bee spraying
        private IEnumerator SprayingRoutine()
        {
            _soundSystem.PlaySound(SPRAY);

            while (_isSpraying)
            {
                ShowSprayFx();
                yield return new WaitForSeconds(1);
            }
        }

        // Show spray particle effect
        private void ShowSprayFx()
        {
            Instantiate(_sprayFx, _sprayPoint.position, Quaternion.identity, transform);
        }

        // Hide the hint pointer
        private void HideHint()
        {
            HintSystem.Instance.HidePointerHint();
        }

    }
}