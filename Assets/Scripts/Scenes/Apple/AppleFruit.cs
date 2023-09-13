using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Apple
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class AppleFruit : MonoBehaviour, ICollectable
    {
        private const string Success = "Success";
        private const string TAKE = "Take";
        private const string PUT = "Put";

        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _onPickUpScaleKoef;
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private SpriteRenderer _redApple;

        private bool _isStored = false;
        private bool _firstClick = false;

        private int _sortingLayerID;

        private InputSystem InputSystem { get; set; }
        private SoundSystem SoundSystem { get; set; }
        public SpriteRenderer Renderer { get; private set; }
        private AppleHole Hole { get; set; }

        private UnityEngine.Transform _fallTransform;

        public event Action OnAppleRipe;
        public event Action OnAppleGrew;
        
        public bool IsRipe => Math.Abs(transform.localScale.x) >= 0.3f;
        public bool IsGrown => Math.Abs(transform.localScale.x) >= 0.2f;

        private Vector3 CachedScale { get; set; } = Vector3.one;

        private Vector3 _lastPosition;
        private Vector3 _randomPositionAroundHole;
        private Vector3 RandomPositionAroundHole
        {
            get
            {
                if (_randomPositionAroundHole == null || _randomPositionAroundHole == Vector3.zero)
                    GetRandomPositionAroundHole();
                return _randomPositionAroundHole;
            }
            set
            {
                _randomPositionAroundHole = value;
            }
        }


        // Subscribe to the OnDragStart event of the DragAndDrop component
        private void Awake()
        {
            _sortingLayerID = SortingLayer.NameToID("Product");
            Renderer = _redApple;
            transform.localScale = Vector3.zero;

            _dragAndDrop.OnDragStart += MakeBiggerOnDrag;
            _dragAndDrop.OnDragEnded += MoveToHolePosiion;
            _dragAndDrop.OnDragEnded += MakeNormalSize;
            _dragAndDrop.OnDragStart += DeactivateHint;
        }

        // Unsubscribe from events to prevent memory leaks
        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= MakeBiggerOnDrag;
            _dragAndDrop.OnDragEnded -= MakeNormalSize;
            _dragAndDrop.OnDragStart -= DeactivateHint;
            _dragAndDrop.OnDragEnded -= MoveToHolePosiion;
        }

        // Construct the apple with the provided parameters
        public void Construct(SoundSystem soundSystem, InputSystem inputSystem, AppleHole hole, UnityEngine.Transform fallTranform)
        {
            SoundSystem = soundSystem;
            InputSystem = inputSystem;
            Hole = hole;
            _fallTransform = fallTranform;
            _dragAndDrop.Construct(InputSystem);
        }

        // Deactivate the hint system
        public void DeactivateHint()
            => HintSystem.Instance.HidePointerHint();

        // Reset the size of the apple to its original scale
        private void MakeNormalSize()
            => transform.DOScale(CachedScale, 0.5f);

        // Increase the size of the apple during drag
        private void MakeBiggerOnDrag()
        {
            if(!_firstClick)
            {
                CachedScale = transform.localScale;
                _firstClick = true;
            }

            PlayTakeSound();
            transform.DOScale(transform.localScale + new Vector3(transform.localScale.x > 0 ? 
            _onPickUpScaleKoef : -_onPickUpScaleKoef, _onPickUpScaleKoef, _onPickUpScaleKoef), 0.5f);
        }

        // Play [PUT] sound
        private void PlayPutSound()
            => SoundSystem.PlaySound(PUT);
        
        // Play [TAKE] sound
        private void PlayTakeSound()
            => SoundSystem.PlaySound(TAKE);

        // Move the apple to the hole's position, also calcualte distance between "_lastPosition" and current position,
        // if distance more than 1 call PlayPutSound() 
        private void MoveToHolePosiion()
        {   
            if (Vector3.Distance(transform.position, _lastPosition) >= 1f)
            {
                DOVirtual.DelayedCall(0.75f, () =>
                {
                    PlayPutSound();
                });
            }

            transform.DOMove(_fallTransform.position, 0.9f).OnComplete(() => 
            {
                _lastPosition = transform.position;
            });
            transform.DORotate(_fallTransform.rotation.eulerAngles, .5f);
        }

        // Make the apple interactable
        public void MakeInteractable()
        {
            _collider.enabled = true;
            _dragAndDrop.IsDraggable = true;
            SetSortingLayer();
        }

        // Set the sorting layer of the apple and its renderer
        private void SetSortingLayer()
        {
            _redApple.sortingLayerID = _sortingLayerID;
            _renderer.sortingLayerID = _sortingLayerID;
        }

        // Make the apple non-interactable
        public void MakeNonInteractable()
        {
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
        }

        // Perform actions when the apple is stored
        public void Stored()
        {
            SoundSystem.PlaySound(Success);
            _isStored = true;
            MakeNonInteractable();
        }

        // Grow the apple with animation
        public void Grow(float endValue, float duration, float extraInterval, int needToReturn)
        {
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(1f + extraInterval);
            sequence.Append(transform.DOScaleX(needToReturn * endValue, duration)
            .OnPlay(() => transform.DOScaleY(endValue, duration))
            .OnStart(() => SoundSystem.PlaySound("Bubble")));
            sequence.AppendCallback(() => OnAppleGrew?.Invoke());
        }

        // Perform actions when the apple is ripe
        internal void Ripe(float ripeDuration)
        {
            _renderer.material.DOFade(0, ripeDuration).OnComplete(() => _renderer.enabled = false);
            transform.DOScaleX(transform.localScale.x > 0 ? 0.3f : -0.3f, ripeDuration);
            transform.DOScaleY(0.3f, ripeDuration);
        }

        // Get a random position around the apple hole
        private void GetRandomPositionAroundHole()
        {
            var randX = UnityEngine.Random.Range(-0.5f, 0.5f);
            var randY = UnityEngine.Random.Range(-0.5f, 0f);
            RandomPositionAroundHole = Hole.transform.position + new Vector3(randX, randY, 0);
        }

        // Set the visual part of the apple with the specified sorting index and layer
        public void MakeVisualPartOf(SpriteRenderer frontBasketSprite, int sortingIndex, int sortingLayer)
        {
            _redApple.sortingLayerID = sortingLayer;
            _redApple.sortingOrder = sortingIndex;
            _renderer.sortingLayerID = sortingLayer;
            _renderer.sortingOrder = sortingIndex;
        }
    }
}