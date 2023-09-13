using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UI;
using UnityEngine;
using UsefulComponents;

namespace Sheep
{
    public class FurBasket : MonoBehaviour, IWinInvoker
    {
        private const string SUCCESS_CONST = "Success";

        public event Action OnWin;

        [SerializeField] private SheepLevelConfig _config;
        [SerializeField] private float _movingDuration;        
        [SerializeField] private ComposedFurTriggerObserver _triggerObserver;
        [SerializeField] private Transform[] _storePoints;
        [SerializeField] private SpriteRenderer _basketFrontSprite;

        private Vector3 _spawnPoint;
        private Vector3 _destination;
        private int _storedFurCount;
        private int _step;
        private FxSystem _fxSystem;
        private SoundSystem _soundSystem;
        private ArrowController _arrowController;

        //constructs FurBasket 
        public void Construct(Vector3 spawnPoint, Vector3 destination, FxSystem fxSystem, SoundSystem soundSystem, ArrowController arrowController)
        {
            _spawnPoint = spawnPoint;
            _destination = destination;
            _fxSystem = fxSystem;
            _soundSystem = soundSystem;
            _arrowController = arrowController;
            _triggerObserver.OnTriggerEnter += ProcessFur;
            MoveToDestinationPoint().OnComplete(ActivateFurBasketArrow);
        }

        // It unsubscribes from events
        private void OnDestroy()
            => _triggerObserver.OnTriggerEnter -= ProcessFur;

        // Activate the fur basket arrow by showing it and animating its movement
        private void ActivateFurBasketArrow()
        {
            _arrowController.ShowArrow();
            DOTween.Sequence().Append(_arrowController.MoveAheadArrow()).SetLoops(-1, LoopType.Yoyo);
        }

        // Process a composed fur by storing it
        private void ProcessFur(ComposedFur fur)
        {
            fur.Stored();
            MakeFurVisualChildOfBasket(fur);

            fur.transform.DOMove(CalculatePointInBasket(), _movingDuration)
                .OnComplete(() => StoreFur(fur));
            NextStep();
        }

        // Make the fur's visual sprites a child of the basket, adjusting their sorting order
        private void MakeFurVisualChildOfBasket(ComposedFur fur)
        {
            foreach (var sprite in fur.FurSprites)
            {
                sprite.sortingOrder = _basketFrontSprite.sortingOrder - 1;
            }
        }

        // Proceed to the next step in the process, and reset to the beginning if the maximum step is reached
        private void NextStep()
        {
            _step++;

            if (IsMaxStep())
            {
                _step = 0;
            }
        }

        // Store the fur permanently by setting its parent to the basket, updating the stored fur count
        private void StoreFur(ComposedFur fur)
        {
            fur.transform.SetParent(transform);
            _storedFurCount++;
            _fxSystem.PlayEffect(SUCCESS_CONST, fur.transform.position);
            _soundSystem.PlaySound(SUCCESS_CONST);
            Debug.Log(_storedFurCount);
            HideHint();

            if (BasketIsFull())
            {
                MoveToSpawnPoint().OnComplete(() => OnWin?.Invoke());
                _arrowController.HideArrow();
            }
        }

        // Check if the current step is the maximum step
        private bool IsMaxStep()
            => _step == _storePoints.Length;

        // Calculate the position in the basket for the current step
        private Vector3 CalculatePointInBasket()
            => _storePoints[_step].position;

        // Check if the basket is full based on the stored fur count
        private bool BasketIsFull()
            => _storedFurCount >= _config.ComposedFurToSpawn;

        // Move the basket to the destination point using a tween animation
        private Tween MoveToDestinationPoint()
            => transform.DOMove(_destination, _movingDuration);

        // Move the basket to the spawn point using a tween animation
        private Tween MoveToSpawnPoint()
            => transform.DOMove(_spawnPoint, _movingDuration);

        // Hide the hint for the pointer
        private void HideHint()
        {
            HintSystem.Instance.HidePointerHint();
        }

    }
}