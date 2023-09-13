using DG.Tweening;
using AwesomeTools.Sound;
using AwesomeTools;
using AwesomeTools.Inputs;
using System;
using UnityEngine;
using UsefulComponents;
using Fx;

namespace CowScene
{
    public class Hay : MonoBehaviour
    {
        private const string SUCCESS = "Success";
        private const string BOOBLE = "Booble";
        private const string PUT = "Put";
        [SerializeField] private Effect _successFx;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private float _movingDuration;
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        private ISoundSystem _soundSystem;

        public event Action<bool, Hay> OnDrag;

        // Constructs the hay object with the specified start point, spawn point
        public void Construct(Vector3 startPoint, Vector3 spawnPoint, ISoundSystem soundSystem)
        {
            transform.position = spawnPoint;
            _soundSystem = soundSystem;
            _destinationOnDragEnd.Construct(startPoint);
            _destinationOnDragEnd.OnMoveComplete += () => SetInteractable(true);
            _dragAndDrop.OnDragStart += StartDragging;
            _dragAndDrop.OnDragEnded += () => SetInteractable(false);
            _dragAndDrop.OnDragEnded += HayDropped;
        }

        // It unsubscribes from events
        private void OnDisable()
        {
            _destinationOnDragEnd.OnMoveComplete -= () => SetInteractable(true);
            _dragAndDrop.OnDragStart -= StartDragging;
            _dragAndDrop.OnDragEnded -= () => SetInteractable(false);
            _dragAndDrop.OnDragEnded -= HayDropped;
        }

        // Handles the start of dragging the hay object
        private void StartDragging()
        {
            OnDrag?.Invoke(true, this);
            HintSystem.Instance.HidePointerHint();
            _soundSystem.PlaySound(BOOBLE);
        }

        // Handles the completion of dropping the hay object
        private void HayDropped()
        {
            OnDrag?.Invoke(false, this);
            DOVirtual.DelayedCall(0.2f, () =>
            {
                _soundSystem.PlaySound(PUT);
            });
            
        }

        // Hides the hay object
        public void Hide()
        {
            _spriteRenderer.enabled = false;
            SetInteractable(false);
            _dragAndDrop.IsDraggable = false;
            gameObject.SetActive(false);
        }

        // Marks the hay object as eaten
        public void GetEaten()
        {
            ShowSuccess();
            OnDrag?.Invoke(false, this);
        }

        // Adds the specified order value to the rendering order of the hay object
        public void AddRenderOrder(int order)
        {
            _spriteRenderer.sortingOrder += order;
        }

        // Shows the success effect and plays the success sound
        public void ShowSuccess()
        {
            _soundSystem.PlaySound(SUCCESS);
            ShowSuccessFx();
        }

        // Instantiates and shows the success effect
        private void ShowSuccessFx()
        {
            Instantiate(_successFx, transform.position, Quaternion.identity);
        }

        // Sets the interactability of the hay object
        public void SetInteractable(bool isInteractable)
        {
            _dragAndDrop.IsDraggable = isInteractable;
            _collider.enabled = isInteractable;
        }

        // Moves the hay object to the specified point and returns the Tween object for animation control
        public Tween MoveTo(Vector3 point)
        {
            return transform.DOMove(point, _movingDuration);
        }



    }
}
