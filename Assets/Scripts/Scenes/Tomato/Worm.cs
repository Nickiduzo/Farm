using DG.Tweening;
using AwesomeTools.Inputs;
using UnityEngine;
using UsefulComponents;
using AwesomeTools.Sound;
using AwesomeTools;

namespace Tomato
{
    public class Worm : MonoBehaviour
    {
        public static int UncatchedWormCount;
        private const string crawlingSound = "Crawling";
        private const string animationName = "Drag";
        private const string animationOnScene = "OnScene";
        
        [SerializeField] private Collider2D _selfCollider;
        [SerializeField] private Animator _animator;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private RandomMover _randomMover;

        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDrag;
        
        private SoundSystem _soundSystem;
        private Vector3 _cachedScale;
        private Vector3 _destinationPoint;
        private int _nonDragSortingOrder;

        public bool _onAdditionalAction;

        /// <summary>
        /// Вводимо систему вводу [inputSystem] - присвоє систему вводу до "DragAndDrop"  та
        /// кінцеву точку для скрипту "MoveToDestinationOnDragEnd" [_destinationOnDrag]
        /// </summary>
        public void Construct(InputSystem inputSystem, SoundSystem soundSystem)
        {
            _soundSystem = soundSystem;
            _dragAndDrop.Construct(inputSystem);
            _destinationPoint = transform.position;
            _destinationOnDrag.Construct(_destinationPoint);
            _destinationOnDrag.OnMoveComplete += MakeInteractable;
            WormsBasket.OnOneWormLeft += () => _onAdditionalAction = true;
            _onAdditionalAction = false;
        }

        /// <summary>
        /// Запа'ятовує розмір елементу і присвоює ф-ції до подій "DragAndDrop"
        /// </summary>
        private void Awake()
        {
            
            _cachedScale = transform.localScale;
            transform.localScale = Vector3.zero;

            _dragAndDrop.OnDragStart += StopCrawlingSound;
            _dragAndDrop.OnDragStart += DisableMoving;
            _dragAndDrop.OnDragStart += SetDragSortingOrder;
            _dragAndDrop.OnDragStart += () => ChangeDragAnimation(true);
            
            
            _dragAndDrop.OnDragEnded += MakeNonInteractable;
            _dragAndDrop.OnDragEnded += SetUnDragSortingOrder;
            _dragAndDrop.OnDragEnded += EnableMoving;
        }

        /// <summary>
        /// Видаляє ф-ції з подій "DragAndDrop"
        /// </summary>
        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= DisableMoving;
            _dragAndDrop.OnDragStart -= SetDragSortingOrder;
            _dragAndDrop.OnDragStart -= () => ChangeDragAnimation(true);
            
            _dragAndDrop.OnDragEnded -= MakeNonInteractable;
            _dragAndDrop.OnDragEnded -= SetUnDragSortingOrder;
            _dragAndDrop.OnDragEnded -= EnableMoving;
        }

        /// <summary>
        /// Вводимо порядок в слою [nonDragSortingOrder] - запам'ятовує порядок слою в полі "_nonDragSortingOrder"
        /// </summary>        
        public void SetNonDragSortingOrder(int nonDragSortingOrder)
        {
            _nonDragSortingOrder = nonDragSortingOrder;
        }

        /// <summary>
        /// Присвоює значення "position.y" до скрипту "RandomMover"
        /// </summary>
        public void SetYToMover()
        {
            _randomMover.InitialPosY();
            _animator.SetBool(animationOnScene, true);
        }

        /// <summary>
        /// Відтворює звук повзання "crawlingSound"
        /// </summary>
        private void PlayCrawlingSound()
        {
            _soundSystem.PlaySound(crawlingSound);
        } 
        
        /// <summary>
        /// Зупиняє звук повзання "crawlingSound"
        /// </summary>
        public void StopCrawlingSound()
        {
            UncatchedWormCount--;
            if(UncatchedWormCount < 1)
            {
                _soundSystem.StopSound(crawlingSound);
            }
        }

        /// <summary>
        /// Забороняє взаємодію з елементом і зупиняє елемент
        /// </summary>
        public void MakeNonInteractable()
        {
            DOTween.Kill(gameObject);
            _dragAndDrop.IsDraggable = false;
            _selfCollider.enabled = false;
            ChangeDragAnimation(false);
            DisableMoving();
        }

        /// <summary>
        /// Дозволяє взаємодію з елементом
        /// </summary>
        public void MakeInteractable()
        {
            UncatchedWormCount++;
            if(_onAdditionalAction)
            {
                PlayCrawlingSound();
            }
            _selfCollider.enabled = true;
            _dragAndDrop.IsDraggable = true;
            ChangeDragAnimation(false);
        }

        /// <summary>
        /// Вводимо порядок в слою [_indexSortingOrder] - присвоює спрайту елемента [_sprite] порядок в слою
        /// </summary>        
        public void SetVisualNonInteractOrder(int _indexSortingOrder)
        {
            _sprite.sortingOrder = _indexSortingOrder;
            _sprite.DOFade(0.7f, 1).SetLink(_sprite.gameObject);
        }

        /// <summary>
        /// Вводимо булеве значення [value]- присвоює булеве значення до аніматора елементу [_animator]
        /// </summary>        
        public void ChangeDragAnimation(bool value)
        {
            HintSystem.Instance.HidePointerHint();
            _animator.SetBool(animationName, value);
        }

        /// <summary>
        /// Показує елемент
        /// </summary>
        public void Appear()
        {
            transform.DOScale(_cachedScale, 1f).SetLink(gameObject).OnComplete(SetYToMover);
            DOVirtual.DelayedCall(0.5f, () => 
            {
                PlayCrawlingSound();
            }
            );
            
        }

        /// <summary>
        /// Присвоює порядок слою,коли тримаєш елемент
        /// </summary>
        private void SetDragSortingOrder()
            => _sprite.sortingOrder = 16;
        
        /// <summary>
        /// Присвоює порядок слою, коли не тримаєш елемент
        /// </summary>
        private void SetUnDragSortingOrder()
            => _sprite.sortingOrder = _nonDragSortingOrder;

        /// <summary>
        /// Забороняє рух елементу
        /// </summary>
        private void DisableMoving()
        {
            _randomMover.enabled = false;
        }

        /// <summary>
        /// Дозволяє рух елементу
        /// </summary>
        private void EnableMoving()
            => _randomMover.enabled = true;
    }
}