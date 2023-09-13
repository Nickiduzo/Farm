using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using AwesomeTools;
using UnityEngine;

namespace Sheep
{
    public class Trimmer : MonoBehaviour
    {
        public SheepAnimator _sheepAnimator;
        [SerializeField] private TrimmerBlade _blade;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private float _movingDuration;
        private Vector3 _startPoint;
        private Tween _movingToStartPoint;
        private Vector3 _spawnPoint;

        // It subscribes to events
        private void Awake()
        { 
            _dragAndDrop.OnDragStart += GainAccessToShowTongue;
            _dragAndDrop.OnDragEnded += MoveToStartPoint;
            _dragAndDrop.OnDragEnded += CancelAccessToShowTongue;
        }
        // It unsubscribes from events
        private void OnDestroy() 
        {
            _dragAndDrop.OnDragStart -= GainAccessToShowTongue;
            _dragAndDrop.OnDragEnded -= MoveToStartPoint;
            _dragAndDrop.OnDragEnded -= CancelAccessToShowTongue;
        }

        //Constructs Trimmer
        public void Construct(Vector3 startPoint, Vector3 spawnPoint, InputSystem inputSystem, SoundSystem soundSystem, FxSystem fxSystem)
        {
            _spawnPoint = spawnPoint;
            _dragAndDrop.Construct(inputSystem);
            _blade.Construct(soundSystem, fxSystem);
            _startPoint = startPoint;
        }

        // Cancel access to show tongue animation
        private void CancelAccessToShowTongue()
        {
            if (IsSheepAnimatorNull()) return;
            SheepAnimator.IsTrimmerDragged = false;
            _sheepAnimator.PlayIdle();
        }

        // Gain access to show tongue animation
        public void GainAccessToShowTongue()
        {
            if (IsSheepAnimatorNull()) return;
            SheepAnimator.IsTrimmerDragged = true;
            _sheepAnimator.PlayTrimLastFur();
        }

        // Check if the sheep animator is null
        private bool IsSheepAnimatorNull()
        {
            return _sheepAnimator == null;
        }

        // Set the sheep animator
        public void SetAnimator(SheepAnimator sheepAnimator)
        {
            _sheepAnimator = sheepAnimator;
        }

        // Move the object to the start point
        public void MoveToStartPoint()
            => _movingToStartPoint = transform.DOMove(_startPoint, _movingDuration);

        // End the life cycle of the object
        public void EndLifeCycle()
        {
            var sequence = DOTween.Sequence().SetLink(gameObject);
            sequence.AppendCallback(Disable);
            sequence.Append(MoveToSpawnPoint());
        }

        // Move the object to the spawn point
        private Tween MoveToSpawnPoint()
            => transform.DOMove(_spawnPoint, _movingDuration);

        // Disable the object
        private void Disable()
        {
            _dragAndDrop.OnDragEnded -= MoveToStartPoint;
            _dragAndDrop.IsDraggable = false;
            _movingToStartPoint.Kill();
        }

        // Make the object non-interactable
        public void MakeNonInteractable()
        {
            _blade.IsTrimmerHasAnimator = false;
            _dragAndDrop.IsDraggable = false;
            MoveToStartPoint();
        }

        // Make the object interactable
        public void MakeInteractable()
        {
            _dragAndDrop.IsDraggable = true;
        }

    }
}