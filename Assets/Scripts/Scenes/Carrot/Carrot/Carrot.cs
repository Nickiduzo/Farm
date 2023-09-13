using DG.Tweening;
using AwesomeTools.Inputs;
using System;
using UnityEngine;
using AwesomeTools;

namespace Carrot
{
    public class Carrot : MonoBehaviour, ICollectable
    {
        public event Action Dragging;

        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private SpriteRenderer _carrotSpriteRenderer;
        [SerializeField] private SpriteRenderer _greenSpriteRenderer;
        [SerializeField] private Animation _greenAnimation;

        private Collider2D _collider;
        private int _sortingLayerID;

        // set sorting layer "Product"
        public void Awake()
        {
            _collider = GetComponent<Collider2D>();
            _sortingLayerID = SortingLayer.NameToID("Product");
        }
        
        // set up such scripts as: "DragAndDrop", "MoveToDestinationOnDragEnd" and make the carrot unavailble for tapping
        public void Construct(InputSystem inputSystem)
        {
            _dragAndDrop.Construct(inputSystem);
            _destinationOnDragEnd.Construct(transform.position);
            MakeNonInteractable();
        }

        // monitor whether carrot has been picked, if so, invoke [FirstDrag] and make availble for dragging
        public void MakeInteractable()
        {
            _collider.enabled = true;
            _dragAndDrop.IsDraggable = true;
            _dragAndDrop.enabled = true;
            _destinationOnDragEnd.enabled = true;
            _dragAndDrop.OnDragStart += FirstDrag;
        }
        
        // stop all Tweens and make unavailble for dragging
        public void MakeNonInteractable()
        {
            DOTween.Kill(gameObject);
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
            _dragAndDrop.enabled = false;
            _destinationOnDragEnd.enabled = false;
        }

        // set necessary Sorting Order and Sorting Layer for collecting in basket 
        // (we do this for avoid cutting front and back of basket sprite when product move to basket)
        private void FirstDrag()
        {
            _greenAnimation.Stop();
            Dragging?.Invoke();
            SetSortingOrder(18);
            SetSortingLayer();
            _dragAndDrop.OnDragStart -= FirstDrag;
        }

        // set new Sorting Order/Layer for two parts of carrot (orange and green)
        private void SetSortingOrder(int sortingIndex)
        {
            _carrotSpriteRenderer.sortingOrder = sortingIndex;
            _greenSpriteRenderer.sortingOrder = _carrotSpriteRenderer.sortingOrder - 1;
        }
        private void SetSortingLayer()
        {
            _carrotSpriteRenderer.sortingLayerID = _sortingLayerID;
            _greenSpriteRenderer.sortingLayerID = _sortingLayerID;
        }

        // get [_sortingIndex] and [_sortingLayer] from basket and set them for all product sprites
        public void MakeVisualPartOf(SpriteRenderer _frontBasketSprite, int _sortingIndex, int _sortingLayer)
        {
            _carrotSpriteRenderer.sortingLayerID = _sortingLayer;
            _carrotSpriteRenderer.sortingOrder = _sortingIndex;
            _greenSpriteRenderer.sortingLayerID = _sortingLayer;
            _greenSpriteRenderer.sortingOrder = _sortingIndex - 1 ;
        }
    }
}