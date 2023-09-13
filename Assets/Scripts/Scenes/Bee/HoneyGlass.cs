using DG.Tweening;
using AwesomeTools.Sound;
using AwesomeTools;
using UnityEngine;
using UsefulComponents;

namespace Bee
{
    public class HoneyGlass : MonoBehaviour, IHoneyGlass, ICollectable
    {
        private const string SUCCESS = "Success";
        private const string OPEN = "Open";
        
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private SpriteRenderer _glassRenderer;
        [SerializeField] private SpriteRenderer _liquidRenderer;
        [SerializeField] private SpriteRenderer _circleRenderer;
        [SerializeField] private SpriteRenderer _bgRenderer;
        [SerializeField] private Transform _liquidPivot;
        [SerializeField] private MoveStartDestination _move;
        [SerializeField] private Cover _cover;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private DragAndDrop _dragAndDrop;
        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;
        private int _sortingLayerID;

        //Constructs honeyGlass
        public void Construct(ISoundSystem soundSystem, FxSystem fxSystem)
        {
            _sortingLayerID = SortingLayer.NameToID("Product");
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
        }
        // Adjusts the visual order of the glass and liquid based on the sorting order of the provided renderer
        public void MakeVisualPartOf(SpriteRenderer renderer)
        {
            _glassRenderer.sortingOrder = renderer.sortingOrder - 1;
            _liquidRenderer.sortingOrder = renderer.sortingOrder - 2;
        }

        // Animates the glass cover opening
        public Tween Open()
        {
            _soundSystem.PlaySound(OPEN);
            return _cover.Open();
        }

        // Animates the glass cover closing
        public Tween Close()
        {
            return _cover.Close();
        }

        // Enables interactivity of the glass
        public void MakeInteractable()
        {
            _collider.enabled = true;
            _dragAndDrop.IsDraggable = true;
            SetSortingLayer();
        }

        // Sets the sorting layer of the glass and its components
        private void SetSortingLayer()
        {
            _glassRenderer.sortingLayerID = _sortingLayerID;
            _liquidRenderer.sortingLayerID = _sortingLayerID;
            _circleRenderer.sortingLayerID = _sortingLayerID;
            _bgRenderer.sortingLayerID = _sortingLayerID;
        }

        // Stores the glass at the specified point with a given duration
        // Adjusts the scale and position of the glass during the storage animation
        public Tween Store(Transform point, float duration = 1f)
        {
            transform.SetParent(point);
            transform.DOScale(transform.localScale / 1.05f, 0.5f);
            return transform.DOMove(point.position, duration);
        }

        // Initializes the drag end position of the glass
        public void InitOnDragEnd()
        {
            _destinationOnDragEnd.Construct(transform.position);
        }

        // Disables interactivity of the glass
        public void MakeNonInteractable()
        {
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
        }

        // Sets the destination for the glass movement
        public void SetDestination(Vector3 destination)
        {
            _move.Construct(destination, transform.position);
        }

        // Fills the glass with honey using animations
        // Adjusts the scale of the liquid pivot to simulate filling
        public Tween FillWithHoney()
        {
            _liquidPivot.DOScaleX(1, 0.45f);
            return _liquidPivot.DOScaleY(1, 2f);
        }

        // Shows a success effect and plays a sound when the glass is successfully filled
        public void ShowSuccess()
        {
            _soundSystem.PlaySound(SUCCESS);
            _fxSystem.PlayEffect(SUCCESS, transform.position);
        }

        // Moves the glass to its destination position with an opening animation
        public Tween GoToDestination()
        {
            return _move.MoveToDestination()
                .OnComplete(() => 
                {
                    Open();
                });
        }

        // Adjusts the visual order of the glass components based on the sorting index and layer
        public void MakeVisualPartOf(SpriteRenderer frontBasketSprite, int sortingIndex, int sortingLayer)
        {
            _glassRenderer.sortingOrder = sortingIndex + 2;
            _liquidRenderer.sortingOrder = sortingIndex + 1;
            _circleRenderer.sortingOrder = sortingIndex + 3;
            _bgRenderer.sortingOrder = sortingIndex;
            _glassRenderer.sortingLayerID = sortingLayer;
            _liquidRenderer.sortingLayerID = sortingLayer;
            _circleRenderer.sortingLayerID = sortingLayer;
            _bgRenderer.sortingLayerID = sortingLayer;
        }

    }
}