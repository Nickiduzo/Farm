using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using AwesomeTools;
using UsefulComponents;

namespace CowScene
{
    public class Jar : MonoBehaviour, ICollectable
    {
        private const string PRODUCT = "Product";
        private const string MILK_STREAM = "MilkStream";
        [SerializeField] private float _liquidScaleYValue;

        public event Action<Jar> JarFilled;
        public event Action<Jar> JarEmptied;

        [Header("Components")]
        [SerializeField] private SpriteRenderer[] _sprites;
        [SerializeField] private SortingGroup _sortingGroup;
        [SerializeField] private Collider2D _collider;
        [Header("Systems")]
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [Header("Misc")]
        [SerializeField] private float _destinationOffset;
        [SerializeField] private float _movingDuration;
        [SerializeField] private Transform _liquidPivot;
        [SerializeField] private MilkJarCover _cover;

        private ISoundSystem _soundSystem;
        private Vector3 _startPoint;
        private Vector3 _rotatePoint = new Vector3(0f, 0f, 100f);

        private int _sortingLayerID;

        // It Init sorting layer
        private void Awake()
        {
            _sortingLayerID = SortingLayer.NameToID(PRODUCT);
        }

        // Makes the jar interactable by enabling collider, draggable, and destination on drag end
        public void MakeInteractable()
        {
            _collider.enabled = true;
            _dragAndDrop.IsDraggable = true;
            _destinationOnDragEnd.enabled = true;
        }

        // Makes the jar non-interactable by disabling collider, draggable, and destination on drag end
        public void MakeNonInteractable()
        {
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
            _destinationOnDragEnd.enabled = false;
        }

        // Sets the sorting layer for all sprites and sorting group of the jar
        public void SetSortingLayer()
        {
            foreach (var sprite in _sprites)
            {
                sprite.sortingLayerID = _sortingLayerID;
            }
            _sortingGroup.sortingLayerID = _sortingLayerID;
        }

        // Initializes the jar with the specified start point, spawn point
        public void Construct(Vector3 startPoint, Vector3 spawnPoint, ISoundSystem soundSystem)
        {
            _startPoint = startPoint;
            transform.position = spawnPoint;
            _soundSystem = soundSystem;
            _destinationOnDragEnd.Construct(startPoint);
        }

        // Fills the jar with milk by scaling the liquid pivot
        public Tween FillWithMilk()
        {
            return _liquidPivot.DOScaleY(_liquidScaleYValue, 2.5f).OnComplete(() =>
            {
                Close().OnComplete(() =>
                {
                    JarFilled?.Invoke(this);
                    var newEndPoint = new Vector3(-_startPoint.x + _destinationOffset, _startPoint.y, _startPoint.z);
                    MoveTo(newEndPoint);
                });
            });
        }

        // Empties out the jar by scaling the liquid pivot, adjusting sorting order, playing animations, and triggering events
        public Tween EmptyOutJar(Vector3 pos)
        {
            return _liquidPivot.DOScaleY(0, 2f).OnComplete(() =>
            {
                MoveInSortingOrder(0);
                _soundSystem.StopSound(MILK_STREAM);
                RevertSpillAnimation().OnComplete(() =>
                {
                    MoveTo(pos);
                });
                JarEmptied?.Invoke(this);
            });
        }

        // Plays the spill animation by rotating and moving the jar
        public void SpillAnimation()
        {
            transform.DORotate(_rotatePoint, 2f);

            var sequence = DOTween.Sequence();
            sequence.Append(transform.DOLocalMoveY(1f, 1f));
            sequence.Append(transform.DOLocalMoveY(1.15f, 1f));
            sequence.Play();
        }

        // Sets the visual part of the basket with the specified front sprite, sorting index, and sorting layer
        public void MakeVisualPartOf(SpriteRenderer frontBasketSprite, int sortingIndex, int sortingLayer)
        {
            foreach (var sprite in _sprites)
            {
                sprite.sortingLayerID = sortingLayer;
            }
            _sortingGroup.sortingLayerID = sortingLayer;
            _sortingGroup.sortingOrder = sortingIndex;
        }

        // Moves the object to the specified point using DOTween
        public Tween MoveTo(Vector3 point)
            => transform.DOMove(point, _movingDuration);

        // Reverts the spill animation by rotating the object back to zero rotation
        private Tween RevertSpillAnimation()
            => transform.DORotate(Vector3.zero, 1f);

        // Called when the mouse button is pressed down on the object. Hides the pointer hint
        private void OnMouseDown()
            => HintSystem.Instance.HidePointerHint();

        // Opens the cover of the object
        public Tween Open()
            => _cover.Open();

        // Closes the cover of the object
        public Tween Close()
            => _cover.Close();

        // Changes the destination point of the object to the specified destination
        public void ChangeDestinationPoint(Vector3 destination)
            => _destinationOnDragEnd.Construct(destination);

        // Moves the object in the sorting order to the specified layer
        public void MoveInSortingOrder(int layer)
            => _sortingGroup.sortingOrder = layer;

    }
}
