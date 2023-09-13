using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Apple
{
    public class SeedlingsApple : MonoBehaviour, IHintable
    {
        private const string SUCCESS = "Success";

        public event Action OnTreeGrew;
        public event Action OnStored;
        public event Action OnHalfWatering;

        [SerializeField] private List<SeedlingStage> _seedlingStages;

        [SerializeField] private SpriteRenderer renderer;
        [SerializeField] private AppleHoleTriggerObserver _observer;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Collider2D _colliderObserver;
        [SerializeField] private float _timeToGrow;
       
        [field:SerializeField] public SpriteRenderer RootRenderer { get; private set; }
        
        private Vector3 _destination;

        private bool isStored = false;
        private int seedlingStage = 0;
        private SoundSystem SoundSystem { get; set; }
        private Vector3 _holePos { get; set; }
        private Tween _waterHint;
        private GameObject _seedlingsApple;
        private Coroutine _wateringRoutine;


        // It subscribes to events
        private void Awake()
        {
            seedlingStage = 0;
            isStored = false;
            _observer.OnTriggerEnter += ProcessHole;
            _dragAndDrop.OnDragStart += DeactivateHint;
            _dragAndDrop.OnDragEnded += MakeNonInteractable;
            _dragAndDrop.OnDragEnded += () => _colliderObserver.enabled = false;
            _destinationOnDragEnd.OnMoveComplete += MakeInteractable;
        }

        // It unsubscribes from events
        private void OnDestroy()
        {
            _observer.OnTriggerEnter -= ProcessHole;
            _dragAndDrop.OnDragStart -= DeactivateHint;
            _dragAndDrop.OnDragEnded -= MakeNonInteractable;
            _dragAndDrop.OnDragEnded -= () => _colliderObserver.enabled = false;
            _destinationOnDragEnd.OnMoveComplete -= MakeInteractable;
        }

        //init seedlingsApple
        public void Construct(Vector3 destination, InputSystem inputSystem, SoundSystem soundSystem, Vector3 holePos, float timeToDestruction)
        {
            _seedlingsApple = gameObject;
            ContactArea.Instance.WaitingForEndLifecycle(_seedlingsApple, timeToDestruction);
            _destination = destination;
            _dragAndDrop.Construct(inputSystem);
            _destinationOnDragEnd.Construct(destination);
            SoundSystem = soundSystem;
            _holePos = holePos;
        }

        // Processes the seed when it is placed in an apple hole
        private void ProcessHole(AppleHole hole)
        {
            if (isStored) return;

            transform.SetParent(hole.transform);
            MakeNonInteractable();
            ContactArea.Instance.MoveSeedToHole(hole.StorePosition, gameObject.transform, 0.2f).OnComplete(() => hole.StoreSeed(this));
            OnStored?.Invoke();
            isStored = true;
            SoundSystem.PlaySound(SUCCESS);
        }

        // Moves the seed to its destination and activates the hint system
        public Tween MoveToDestination()
        {
            ActivateHint();
            return MoveTo(_destination);
        }

        // Activates the hint for the seed's destination
        public void ActivateHint()
        {
            HintSystem.Instance.ShowPointerHint(_destination, _holePos);
        }

        // Deactivates the hint for the seed
        public void DeactivateHint()
        {
            HintSystem.Instance.HidePointerHint();
        }

        // Activates a water hint for the seed
        public void DoWaterHint()
        {
            _waterHint = renderer.DOColor(Color.red, 1).SetLoops(-1, LoopType.Yoyo);
            _waterHint.Play();
        }

        // Stops the water hint for the seed
        public void StopWaterHint()
        {
            renderer.DOColor(Color.white, 1);
            _waterHint.Kill();
        }

        // Starts the growth process for the seed
        internal void StartGrow()
        {
            _wateringRoutine = StartCoroutine(GrowProcess());
        }

        // Stops the growth process for the seed
        internal void StopGrow()
        {
            StopCoroutine(_wateringRoutine);
        }

        // Makes the seed non-interactable
        private void MakeNonInteractable()
        {
            DOTween.Kill(gameObject);
            _dragAndDrop.IsDraggable = false;
        }

        // Makes the seed interactable
        private void MakeInteractable()
        {
            _dragAndDrop.IsDraggable = true;
            _colliderObserver.enabled = true;
        }

        // Moves the seed to a specific position
        private Tween MoveTo(Vector3 point)
            => transform.DOMove(point, 1f);

        // Moves the seed to a specific position in the hole
        private Tween MoveSeedToHole(Vector3 point)
            => transform.DOMove(point, 0.2f);

        // The growth process for the seed
        // Iterates through each growth stage and waits for a certain amount of time before moving to the next stage
        // Updates the seed's position, scale, and sprite for each stage and triggers the OnHalfWatering event
        // Plays a sound effect when transitioning to each new stage
        private IEnumerator GrowProcess()
        {
            foreach (var stage in _seedlingStages)
            {
                if (!stage.StagePassed)
                {
                    yield return new WaitForSeconds(_timeToGrow);

                    stage.StagePassed = true;
                    ++seedlingStage;

                    var sequence = DOTween.Sequence();

                    sequence.AppendCallback(() => transform.localPosition = stage.Position);
                    sequence.AppendCallback(() => transform.localScale = stage.Scale);
                    sequence.AppendCallback(() => renderer.sprite = stage.Stage);
                    sequence.AppendCallback(() => OnHalfWatering?.Invoke());
                    sequence.AppendCallback(() => SoundSystem.PlaySound("GrowApple"));
                    
                }
            }

            if (seedlingStage >= _seedlingStages.Count - 1)
            {
                var sequence = DOTween.Sequence();
                sequence.AppendInterval(0.5f);
                sequence.AppendCallback(() => _collider.enabled = false);
                sequence.AppendCallback(() => SoundSystem.PlaySound(SUCCESS));
                sequence.AppendCallback(() => OnTreeGrew?.Invoke());
            }
        }

    }
}