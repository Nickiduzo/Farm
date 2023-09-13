using System.Collections.Generic;
using DG.Tweening;
using Fishing.Spawners;
using AwesomeTools.Inputs;
using UnityEngine;
using AwesomeTools;
using AwesomeTools.Sound;

namespace Fishing
{
    public class Fish : MonoBehaviour, ICollectable
    {
        private const string SPLASH = "Splash";
        private readonly int Catch = Animator.StringToHash("Catch");
        public bool IsCatch => _isCatch;
        public bool IsStored => _isStored;
        public bool IsStoredInBasket => _isStoredInBasket;
        public bool IsCollected { get; private set; }
        
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _jumpPower;
        [SerializeField] private RandomMover _randomMover;
        [SerializeField] private RandomSprite _randomSprite;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private float _movingDuration;
        [SerializeField] private int _rotateAngle = 359;

        private bool _isCatch;
        private bool _isStored;
        private bool _isStoredInBasket;

        private int _sortingLayerID;
        
        private Vector3 _positionInNet;
        private SoundSystem _soundSystem;
        private Animator _animator { get; set; }
        public List<SpriteRenderer> Renderers { get; private set; }

        /// <summary>
        /// Виконуємо ф-цію "SetUpFishSprite" та забороняємо взаємодію
        /// </summary>
        private void Awake()
        {
            _sortingLayerID = SortingLayer.NameToID("Product");
            SetUpFishSprite();
            _dragAndDrop.IsDraggable = false;
            _dragAndDrop.OnDragEnded += MoveToNet;
        }

        /// <summary>
        /// Отримуємо випадкову рибу та присвоюємо елементу аниматор [body.Animator] та рендерери [body.SpriteRenderers]
        /// </summary>
        private void SetUpFishSprite()
        {
            var fishVariant = _randomSprite.GetRandomSprite();
            var body = Instantiate(fishVariant, this.transform.position, Quaternion.identity, this.transform);
            _animator = body.Animator;
            Renderers = body.SpriteRenderers;
        }

        /// <summary>
        /// Вводимо систему вводу [inputSystem] та спавнер риби [fishSpawner] - 
        /// присвоюємо "DragAndDrop" систему вводу,
        /// ф-цію "MoveFromScreen" для події "OnFishesDisappear" спавнера риби та
        /// аніматор елементу [Animator] для скрипту "RandomMover" [_randomMover]
        /// </summary>        
        public void Construct(InputSystem inputSystem, FishSpawner fishSpawner, SoundSystem soundSystem)
        {
            _soundSystem = soundSystem;
            _dragAndDrop.Construct(inputSystem);
            fishSpawner.OnFishesDisappear += MoveFromScreen;
            _randomMover.Construct(_animator);
        }

        /// <summary>
        /// Вводимо номер в слою [SortOrderIndex]- 
        /// виконуємо метод "SetUpSpriteOrder" поля "_randomSprite", передаючи параметром номер слою [SortOrderIndex]
        /// </summary>
        public void ChangeSpriteSortOrder(int SortOrderIndex)
        {
            _randomSprite.SetUpSpriteOrder(SortOrderIndex);
        }

        /// <summary>
        /// Перевіряє чи елемент являється складеним та виконує метод "StopOnReachingDestination" поля "_randomMover"
        /// </summary>
        private void MoveFromScreen()
        {
            if (!IsStored)
            {
                _collider.enabled = false;
                gameObject.SetActive(false);
            }
            _randomMover.StopOnReachingDestination();
        }

        /// <summary>
        /// Вводимо позицію [position]- 
        /// присвоюємо позицію елементу [_positionInNet] значення параметру [position]
        /// </summary>
        public void SetPositionInNet(Vector3 position)
            => _positionInNet = position;

        /// <summary>
        /// Переміщає елемент на позицію [_positionInNet] з тривалістю [_movingDuration]
        /// </summary>
        private void MoveToNet()
        {
            if (IsCollected) return;

            transform.DOMove(_positionInNet, _movingDuration);
        }

        /// <summary>
        /// Вимикає переміщення та виконує анімацію "CaughtFIsh" з тривалістю "0,5f"
        /// </summary>
        public void Hooked()
        {
            _randomMover.enabled = false;
            _animator.SetTrigger("Catch");
            _isCatch = true;
        }
        
        /// <summary>
        /// Вимикає колайдер елементу [_collider] та взаємодію з елементом [_dragAndDrop.IsDraggable]
        /// </summary>
        private void MakeNonInteractable()
        {
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
        }
        
        /// <summary>
        /// Присвоює значення "true" полю "_isStored" та прибирає "parent" для елементу
        /// </summary>
        public void Stored()
        {
            _isStored = true;
            transform.SetParent(null);
            SetNewSortingLayer();
        }

        /// <summary>
        /// Вводимо позицію [position] та булеве значення [isRotate] -
        /// повертає елемент, якщо булеве значення [isRotate] "true", та
        /// стрибає на позицію [position] з вказаною силою стрибка [_jumpPower]
        /// </summary>
        public Tween JumpTo(Vector3 position, bool isRotate = true)
        {
            if (isRotate)
                transform.DOLocalRotate(CalculateRotation(), 1f, RotateMode.LocalAxisAdd);

            return transform.DOJump(position, _jumpPower, 1, 1.6f);
        }

        /// <summary>
        /// Повертає значення Vector3 з значенням "rotation.z" елементу
        /// </summary>
        private Vector3 CalculateRotation() => new(0, 0, _rotateAngle);

        private void SetNewSortingLayer()
        {
            Renderers.ForEach(sprite =>
            {
                sprite.sortingLayerID = _sortingLayerID;
            });
        }

        /// <summary>
        /// Вводимо SpriteRenderer [_frontBasketSprite], номер в слою [_sortingIndex] та номер слою [_sortingLayer]-
        /// присвоюємо кожному спрайту елементу зі списку рендерерів [Renderers] номер в слою та номер слою
        /// </summary>
        public void MakeVisualPartOf(SpriteRenderer _frontBasketSprite, int _sortingIndex, int sortingLayerID)
        {
            _animator.enabled = false;
            _soundSystem.PlaySound(SPLASH);
            Renderers.ForEach(sprite =>
            {
                sprite.sortingLayerID = sortingLayerID;
            });
        }
    }
}