using System;
using DG.Tweening;
using UnityEngine;
using AwesomeTools.Sound;
using System.Collections;
using AwesomeTools;

namespace Tomato
{
    public class Tomato : MonoBehaviour, IRipeListener, ICollectable
    {
        private const string TAKE = "Take";
        private const string PUT = "Put";

        public event Action OnGrow;
        private event Action WhileOnGrow;

        [SerializeField] private Collider2D _collider2D;
        [SerializeField] private SpriteRenderer _tomatoSpriteRenderer;
        [SerializeField] private Sprite _redSprite;
        [SerializeField] private float _timeToGrow;
        [Space,SerializeField] private Vector3 _onDragScaleValue;
        [SerializeField] private float _onDragScaleDurationValue;
        [SerializeField] private float _appearSpriteValue;
        [SerializeField] private float _fadeOutSpriteValue;
        [SerializeField] private float _durationOnDragEndValue;

        [SerializeField] private DragAndDrop _dragAndDrop; 
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDrag;

        private Transform _tomatoPool;
        private Vector3 _lastPosition;
        private Vector3 _reservDestinationPoint;
        private SoundSystem _soundSystem;
        private int _sortingLayerID;

        private Quaternion _rotation;

        private Coroutine _putSoundRoutine;

        /// <summary>
        /// Присвоює метод ChangePivot() для події "OnDragStart", обнуляє "transform.scale" елементу,
        /// присвоює ідентифікатор "Product" слою [_sortingLayerID] та виконує метод SetSpriteMaskInteraction()
        /// </summary>
        private void Awake()
        {
            _dragAndDrop.OnDragStart += ChangePivot;
            _dragAndDrop.OnDragStart += PlayTakeSound;
            transform.localScale = Vector3.zero;
            _sortingLayerID = SortingLayer.NameToID("Product");
            SetSpriteMaskInteraction(SpriteMaskInteraction.None);
        }

        public void Construct(SoundSystem soundSystem)
        {
            _soundSystem = soundSystem;
        }


        /// <summary>
        /// Очищає значення події "WhileOnGrow"
        /// </summary>
        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= PlayTakeSound;
            WhileOnGrow = null;
        }

        /// <summary>
        /// Присвоює "transform.rotation" до елементу [transform.localRotation]
        /// </summary>
        private void SetRotationForTomato()
        {
            transform.localRotation = _rotation;
        }

        /// <summary>
        /// Змінює значення "pivot" для спрайту червоного томату та збільшує розмір томату
        /// </summary>
        private void ChangePivot()
        {
            transform.DOScale(_onDragScaleValue, _onDragScaleDurationValue);
            SetParentAsPool();
            Sprite sprite = Sprite.Create(_redSprite.texture, _redSprite.rect, new Vector2(.5f, .5f), _redSprite.pixelsPerUnit);
            _tomatoSpriteRenderer.sprite = sprite;
            _collider2D.offset = Vector2.zero;
            SetRotationForTomato();
            _dragAndDrop.OnDragStart -= ChangePivot;
        }

        /// <summary>
        /// Відтворює звук "TAKE" коли піднімаємо томат
        /// </summary>
        private void PlayTakeSound()
            => _soundSystem.PlaySound(TAKE);
        
        /// <summary>
        /// Відтворює звук "PUT" коли відпускаємо томат
        /// </summary>
        private void PlayPutSound()
            => _soundSystem.PlaySound(PUT);

        /// <summary>
        /// Дозволяє взаємодіяти з томатом
        /// </summary>
        private void MakeInteractable()
        {
            _collider2D.enabled = true;
            _dragAndDrop.IsDraggable = true;
        }
        
        /// <summary>
        /// Забороняє взаємодіяти з томатом
        /// </summary>
        public void MakNonInteractable()
        {
            _dragAndDrop.IsDraggable = false;
            _destinationOnDrag.enabled = false;
            _collider2D.enabled = false;
        }

        /// <summary>
        /// Вводимо подію [action] - присвоюємо подію [action] для події [WhileOnGrow]
        /// </summary>
        public void SetWhileOnGrow(Action action)
            => WhileOnGrow += action;
        
        /// <summary>
        /// Вводимо точку призначення [destinationPoint] - присвоємо події "OnDragEnded" виконання ф-ції MoveOnDragEnded()
        /// </summary>
        /// <param name="destinationPoint">точка призначення</param>
        public void SetReservDestinationPoint(Vector3 destinationPoint)
        {
            _dragAndDrop.OnDragEnded += () => MoveOnDragEnded(destinationPoint);
        }

        /// <summary>
        ///  Вводимо булеве значення [value] - змінюємо значення параметру "IsTrigger" для колайдера елементу [_collider2D]
        /// </summary>
        /// <param name="value">булеве значення</param>
        private void SetEnableStateToCollider(bool value)
            => _collider2D.isTrigger = value;
        
        /// <summary>
        /// Запам'ятовує значення "rotation" елементу [_rotation] та виконує ф-цію OnRipe()
        /// </summary>
        public void Appear(IRipeInvoker invoker)
        {
            _rotation = transform.localRotation;
            OnRipe(invoker);
        }

        /// <summary>
        /// Викликає виконання події "WhileOnGrow" та ф-ції FadeOutSprite(), 
        /// після закінчення якої виконує ф-цію RipeTomato()
        /// </summary>
        public void OnRipe(IRipeInvoker invoker = null)
        {
            WhileOnGrow?.Invoke();
            FadeOutSprite()
                .OnComplete(() => RipeTomato(invoker));
        }

        /// <summary>
        /// Присвоє елементу спрайт червоного томату й викликає ф-цію AppearSprite(), 
        /// після закінчення якої викликає ф-ції MakeInteractable(), SetSortingLayer(),
        /// SetEnableStateToCollider() та подію "OnGrow"
        /// </summary>
        private void RipeTomato(IRipeInvoker invoker)
        {
            _tomatoSpriteRenderer.sprite = _redSprite;
            AppearSprite()
                .OnComplete(() =>
                {
                    invoker.RipedEnded();
                    MakeInteractable();
                    SetSortingLayer();
                    SetEnableStateToCollider(true);
                    OnGrow?.Invoke();
                });
        }

        /// <summary>
        /// Показує спрайт елементу
        /// </summary>
        private Tween AppearSprite()
            => _tomatoSpriteRenderer.DOFade(_appearSpriteValue, _timeToGrow);

        /// <summary>
        /// Ховає спрайт елементу
        /// </summary>        
        private Tween FadeOutSprite()
            => _tomatoSpriteRenderer.DOFade(_fadeOutSpriteValue, _timeToGrow);
        
        /// <summary>
        /// Вводимо точку приземлення [destinationPoint] - рухає елемент до точки приземлення [destinationPoint]
        /// </summary>
        /// <param name="destinationPoint"> точка приземлення</param>
        private void MoveOnDragEnded(Vector3 destinationPoint)
        {
            if (Vector3.Distance(transform.position, _lastPosition) >= 1f)
            {
                DOVirtual.DelayedCall(_durationOnDragEndValue * 0.80f, () =>
                {
                    PlayPutSound();
                });
            }

            if (_reservDestinationPoint == Vector3.zero)
            {
                _reservDestinationPoint = destinationPoint;
            }

            transform.DOMove(_reservDestinationPoint, _durationOnDragEndValue).OnComplete(() =>
            {
                _lastPosition = transform.position;
            });
        }

        /// <summary>
        /// Вводимо значення взаємодії спрайту з масками [interaction]- 
        /// присвоюємо для "SpriteRenderer" елементу значення взаємодії спрайту з масками [_tomatoSpriteRenderer.maskInteraction] 
        /// </summary>
        /// <param name="interaction">значення взаємодії спрайту з масками</param>
        public void SetSpriteMaskInteraction(SpriteMaskInteraction interaction)
            => _tomatoSpriteRenderer.maskInteraction = interaction;

        /// <summary>
        /// Присвоює ідентифікатор слою [_sortingLayerID] елементу [_tomatoSpriteRenderer.sortingLayerID]
        /// </summary>
        private void SetSortingLayer()
            => _tomatoSpriteRenderer.sortingLayerID = _sortingLayerID;

        /// <summary>
        /// Вводимо "transform" пулу томатів [pool] - запам'ятовуємо "transform" пулу томатів в полі _tomatoPool
        /// </summary>
        /// <param name="pool">пул томатів</param>
        public void SetTomatoPool(Transform pool)
            => _tomatoPool = pool;
        
        /// <summary>
        /// Присвоює "parent" для елементу
        /// </summary>
        private void SetParentAsPool()
            => transform.SetParent(_tomatoPool);

        /// <summary>
        /// Вводимо індекс сортування [sortingIndex] та індекс слою [sortingLayer] - 
        /// присвоюємо для компонента елементу "SpriteRender" [_tomatoSpriteRenderer] індекс сортування та індекс слою
        /// </summary>
        /// <param name="_frontBasketSprite">лицева частина кошика</param>
        /// <param name="sortingIndex"> індекс сортування</param>
        /// <param name="sortingLayer"> індекс слою</param>
        public void MakeVisualPartOf(SpriteRenderer _frontBasketSprite, int sortingIndex, int sortingLayer)
        {
            _tomatoSpriteRenderer.sortingOrder = sortingIndex;
            _tomatoSpriteRenderer.sortingLayerID = sortingLayer;
        }
    }
}