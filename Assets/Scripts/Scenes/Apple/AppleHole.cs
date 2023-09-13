using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UsefulComponents;

namespace Apple
{
    public class AppleHole : BaseHole, IDiggable
    {
        public event Action OnHoleReadyToSeed;

        public event Action OnApplesRipe;
        public event Action OnApplesGrew;

        [SerializeField] private SpriteRenderer _dirt;
        [SerializeField] private SpriteRenderer _dirtHalf;
        [SerializeField] private List<Transform> _appleAppearPoints;
        [SerializeField] private Transform _storePosition;
        [SerializeField] private SpriteRenderer _backHole;
        [SerializeField] private float _framesToFill;

        [SerializeField] private SpriteRenderer _seedling;

        public List<Transform> AppleAppearPoints => _appleAppearPoints;
        public Vector3 StorePosition => _storePosition.position;

        private SeedlingsApple _storedSeed;
        private List<Vector3> _appleAppearPositions = new();
        private IChainAction _dirtAction;

        private List<AppleFruit> _apples = new();
        public bool IsApplesRipe { get; private set; } = false;
        public bool IsApplesGrown { get; private set; } = false;
        private Tween _seedLingHint { get; set; }

        //subscribe to the events and init params
        private void Awake()
        {
            IsApplesRipe = false;
            IsApplesGrown = false;
            CalculateAppleSpawnPoints();
            InitActions();
            _progressCalculator = new FrameProgressCalculator(CalculateMaxProgress());
            _progressCalculator.OnProgressStep += ExecuteDigAction;
        }

        //init DirtHalfScale and set new actions to _dirtAction
        private void InitActions()
        {
            _dirtAction = new DirtHalfScale(_dirt);
            _dirtAction.SetNextAction(new DirtDisappear(_dirtHalf, this));
        }

        // Calculate the positions for apple spawn points
        private void CalculateAppleSpawnPoints()
        {
            foreach (var point in _appleAppearPoints)
                _appleAppearPositions.Add(point.position);
        }

        // Execute the dig action
        private void ExecuteDigAction()
            => _dirtAction.Execute();

        // Make the seed part of the hole
        private void MakeSeedPartOfHole(SeedlingsApple seed)
        {
            seed.GetComponent<SpriteRenderer>().sortingOrder = _backHole.sortingOrder;
            seed.RootRenderer.enabled = false;
            _backHole.enabled = false;
        }

        // Calculate the maximum progress
        private float CalculateMaxProgress()
            => Time.deltaTime * _framesToFill;

        // Store the seed in the hole
        public void StoreSeed(SeedlingsApple seed)
        {
            _seedLingHint?.Kill();
            PlaySuccessFX();
            _storedSeed = seed;
            _seedling.gameObject.SetActive(false);
            MakeNonInteractable();
            MakeSeedPartOfHole(seed);
            _storedSeed.OnTreeGrew += GrowApples;
        }

        // Trigger the growth of _apples
        private void GrowApples()
        {
            PlaySuccessFX();

            float extraInterval = 0;
            var isNeedToRotate = false;
            foreach (var apple in _apples)
            {
                extraInterval += 0.3f;
                apple.Grow(0.2f, 1f, extraInterval, isNeedToRotate ? -1 : 1);
                isNeedToRotate = !isNeedToRotate;
            }
        }

        // Dig the hole
        public void Dig()
        {
            OnHoleReadyToSeed?.Invoke();
        }

        // Add an apple to the hole
        public void AddApple(AppleFruit apple)
        {
            _apples.Add(apple);
            apple.OnAppleRipe += CheckAppleRipe;
            apple.OnAppleGrew += CheckAppleGrew;
        }

        // Check if all _apples in the hole are ripe
        private void CheckAppleRipe()
        {
            if (_apples.All(x => x.IsRipe))
            {
                IsApplesRipe = true;
                OnApplesRipe?.Invoke();
            }
        }

        // Check if all _apples in the hole have grown
        private void CheckAppleGrew()
        {
            if (_apples.All(x => x.IsGrown))
            {
                IsApplesGrown = true;
                OnApplesGrew?.Invoke();
            }
        }

        // Trigger the ripening animation for the _apples
        internal void RipeApples(float ripeDuration)
        {
            foreach (var apple in _apples)
            {
                apple.Ripe(ripeDuration);
            }
        }

        // Make the _apples interactable
        public void MakeApplesInteractable()
        {
            foreach (var apple in _apples)
            {
                apple.MakeInteractable();
            }
        }

        // Start the seedling hint animation
        public void StartSeedlingHint()
        {
            _seedLingHint = _seedling.DOFade(0.7f, 1).SetLoops(-1, LoopType.Yoyo);
            _seedLingHint.Play();
        }

        // Start the watering hint animation
        public void StartWateringHint()
        {
            _storedSeed.DoWaterHint();
        }

        // Get a list of the _apples in the hole
        public List<AppleFruit> Apples()
        {
            return _apples;
        } 

        // Activate the hint effect
        public void ActivateHint()
        {
            var color = _dirt.color;
            var sequence = DOTween.Sequence();
            sequence.Append(_dirt.DOColor(Color.red, 1f).SetLoops(3, LoopType.Yoyo));
            sequence.Append(_dirt.DOColor(color, 0.1f));
        }
    }
}