using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using UnityEngine.Rendering;
using AwesomeTools;
using UsefulComponents;

namespace CowScene
{
    public class MilkBottle : MonoBehaviour, ICollectable
    {
        private const string PRODUCT = "Product";
        private const string PLACE_BOTTLE_CAP = "PlaceBottleCap";
        private const string PLACE_BOTTLE = "PlaceBottle";

        public event Action BottleArrived;
        public event Action<MilkBottle> BottleFilled;

        [Header("Systems")]
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [Header("Components")]
        [SerializeField] private Collider2D _collider;
        [Header("Sprites")]
        [SerializeField] private SortingGroup _sortingGroup;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private SpriteRenderer _spriteFront;
        [SerializeField] private SpriteRenderer _spriteMilk;
        [SerializeField] private SpriteRenderer _spriteCover;
        [Header("Misc")]
        [SerializeField] private MilkJarCover _cover;
        [SerializeField] private float _movingDuration;
        [SerializeField] private float _jumpPower;
        [SerializeField] private Transform _liquidPivot;

        private ISoundSystem _soundSystem;
        private int _sortingLayerID;

        //Init MilkBottle and jump to destination
        public void Construct(Vector3 destination, Vector3 jumpDestination, Vector3 spawnPoint, ISoundSystem soundSystem)
        {
            transform.position = spawnPoint;
            _soundSystem = soundSystem;
            _sortingLayerID = SortingLayer.NameToID(PRODUCT);
            JumpTo(destination, jumpDestination).OnComplete(OpenLid);
        }

        // Makes the bottle interactable
        public void MakeInteractable()
        {
            _destinationOnDragEnd.Construct(transform.position);
            _collider.enabled = true;
            _dragAndDrop.IsDraggable = true;
            _destinationOnDragEnd.enabled = true;
            SetSortingLayer();
        }

        // Makes the bottle non-interactable
        public void MakeNonInteractable()
        {
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
            _destinationOnDragEnd.enabled = false;
        }

        // Sets the sorting layer and order for the visual components of the basket
        private void SetSortingLayer()
        {
            _sprite.sortingLayerID = _sortingLayerID;
            _spriteFront.sortingLayerID = _sortingLayerID;
            _spriteMilk.sortingLayerID = _sortingLayerID;
            _spriteCover.sortingLayerID = _sortingLayerID;
            _sortingGroup.sortingLayerID = _sortingLayerID;
            _sortingGroup.sortingOrder = _sortingLayerID;
        }

        // Makes the visual part of the basket based on the provided parameters
        public void MakeVisualPartOf(SpriteRenderer frontBasketSprite, int sortingIndex, int sortingLayer)
        {
            _sprite.sortingLayerID = sortingLayer;
            _spriteFront.sortingLayerID = sortingLayer;
            _spriteMilk.sortingLayerID = sortingLayer;
            _spriteCover.sortingLayerID = sortingLayer;
            _sprite.sortingOrder = sortingIndex;
            _spriteFront.sortingOrder = sortingIndex + 2;
            _spriteMilk.sortingOrder = sortingIndex + 1;
            _spriteCover.sortingOrder = sortingIndex + 3;
            _sortingGroup.sortingLayerID = sortingLayer;
            _sortingGroup.sortingOrder = sortingIndex;
        }

        // Opens the lid, plays the "PlaceBottleCap" sound, and invokes the BottleArrived event
        private void OpenLid()
        {
            Open().OnComplete(() => _soundSystem.PlaySound(PLACE_BOTTLE_CAP));
            InvokeOnArrived();
        }

        // Invokes the BottleArrived event
        private void InvokeOnArrived()
            => BottleArrived?.Invoke();

        // Invokes the BottleFilled event
        private void InvokeBottleFilled()
            => BottleFilled?.Invoke(this);

        // Actions to perform when the basket arrives on the table
        private void ArrivedOnTable()
        {
            SetSortingOrder(15);
            DOVirtual.DelayedCall(0.3f, () =>
            {
                _soundSystem.PlaySound(PLACE_BOTTLE);
            });
        }

        // Sets the sorting order of the basket's visual components
        public void SetSortingOrder(int order)
            => _sortingGroup.sortingOrder = order;

        // Opens the basket
        public Tween Open()
            => _cover.Open();

        // Closes the basket
        public Tween Close()
            => _cover.Close();

        // Moves the basket to the target point
        public Tween MoveTo(Vector3 targetPoint)
            => transform.DOMove(targetPoint, 1f);

        // Jumps the basket to the first point and then moves it to the second point
        private Tween JumpTo(Vector3 point, Vector3 point2)
        {
            return transform.DOJump(point, _jumpPower, 1, _movingDuration)
                .AppendCallback(ArrivedOnTable)
                .Append(transform.DOMove(point2, 0.5f));
        }

        // Handles the mouse down event by hiding the pointer hint
        private void OnMouseDown()
            => HintSystem.Instance.HidePointerHint();


        // Fills the basket with milk and closes the lid
        public Tween FillWithMilk()
            => _liquidPivot.DOScaleY(0.9f, 2f).OnComplete(CloseLid);

        // Closes the lid and invokes the BottleFilled event
        private void CloseLid()
            => Close().OnComplete(InvokeBottleFilled);

    }
}

