using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using AwesomeTools;

namespace Bee
{
    public class HoneyComb : MonoBehaviour, IHoneyComb
    {
        private const string SUCCESS = "Success";
        public event Action OnStored;

        [SerializeField] private SpriteRenderer[] _combFrontSprites;
        [SerializeField] private SpriteRenderer _combBackSprite;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDrag;
        [SerializeField] private float _movingDownDepth;
        [SerializeField] private Animation _RedBlinkAnim;

        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;
        private Vector3 _initialPosition;

        private int _sortingOrder;
        private float _yOffSet = 1.3f;
        private bool _inRecycler = false;

        public bool _availableForRecycler;

        // It init systems
        public void Construct(ISoundSystem soundSystem, FxSystem fxSystem)
        {
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
        }

        // It subscribes from events
        private void Awake()
        {
            _initialPosition = new Vector3(transform.position.x, transform.position.y + _yOffSet, transform.position.z);
            _destinationOnDrag.Construct(_initialPosition);
            _destinationOnDrag.OnMoveComplete += SetVisualNonInteractOrder;
            _destinationOnDrag.OnMoveComplete += MoveDownToHive;
            _dragAndDrop.OnDragStart += SetVisualInteractOrder;
            _dragAndDrop.OnDragStart += MakeAvailableForRecycler;
            _dragAndDrop.OnDragEnded += MakeNonAvailableForRecycler;
            MakeNonInteractable();
        }
        // It unsubscribes from events
        private void OnDestroy()
        {
            _destinationOnDrag.OnMoveComplete -= SetVisualNonInteractOrder;
            _destinationOnDrag.OnMoveComplete -= MoveDownToHive;
            _dragAndDrop.OnDragStart -= SetVisualInteractOrder;
            _dragAndDrop.OnDragStart -= MakeAvailableForRecycler;
            _dragAndDrop.OnDragEnded -= MakeNonAvailableForRecycler;
        }

        // Make the honeycomb available for the recycler
        public void MakeAvailableForRecycler()
        {
            _sortingOrder = 21;
            SpriteSortingOrderChanger(_sortingOrder);
            _availableForRecycler = false;
        }

        // Make the honeycomb non-available for the recycler
        public void MakeNonAvailableForRecycler()
        {
            _availableForRecycler = true;
        }

        // Set the visual interact order for the honeycomb
        public void SetVisualInteractOrder()
        {
            _sortingOrder = 6;
            _soundSystem.PlaySound("Buble");
            SpriteSortingOrderChanger(_sortingOrder);
        }

        // Set the new interact sorting order for the honeycomb
        private void NewInteractSortingOrder()
        {
            _sortingOrder = 21;
            SpriteSortingOrderChanger(_sortingOrder);
        }

        // Set the visual non-interact order for the honeycomb
        private void SetVisualNonInteractOrder()
        {
            _sortingOrder = 1;
            SpriteSortingOrderChanger(_sortingOrder);
        }

        // Change the sorting order of the honeycomb sprites
        private void SpriteSortingOrderChanger(int sortingOrder)
        {
            _combBackSprite.sortingOrder = sortingOrder;
            foreach (var sprite in _combFrontSprites)
            {
                sprite.sortingOrder = sortingOrder + 1;
            }
        }

        // Move the honeycomb down to the hive
        private void MoveDownToHive()
        {
            if (_inRecycler == false)
            {
                transform.DOMoveY(transform.position.y - _yOffSet, 0.5f);
            }
        }

        // Triggered when the honeycomb is recycled
        public void OnRecycled()
        {
            _soundSystem.PlaySound(SUCCESS);
            _fxSystem.PlayEffect(SUCCESS, transform.position);
            gameObject.SetActive(false);
        }

        // Make the honeycomb interactable
        public void MakeInteractable()
        {
            _RedBlinkAnim.Play();
            _collider.enabled = true;
            _dragAndDrop.IsDraggable = true;
        }

        // Make the honeycomb non-interactable
        public void MakeNonInteractable()
        {
            _RedBlinkAnim.clip.SampleAnimation(_RedBlinkAnim.gameObject, 0f);
            _RedBlinkAnim.Stop();
            DOTween.Kill(gameObject);
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
        }

        // Move the honeycomb down per step
        public Tween MoveDownPerStep()
        {
            return transform.DOMoveY(transform.position.y - _movingDownDepth, 0.001f);
        }

        // Store the honeycomb at the pre-store position
        public void Stored(Transform preStorePosition)
        {
            _inRecycler = true;
            _initialPosition = preStorePosition.position;
            _soundSystem.PlaySound("Put");
            MakeNonInteractable();
            NewInteractSortingOrder();
            OnStored?.Invoke();
        }

    }
}