using System.Collections.Generic;
using DG.Tweening;
using AwesomeTools.Inputs;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using AwesomeTools;
using System.Collections;
using AwesomeTools.Sound;

namespace ChickenScene.Entities
{
    public class WormChicken : MonoBehaviour
    {
        private const string crawlingSound = "Crawling";

        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private float _wormSpeed;
        [SerializeField] private Collider2D _collider;

        private List<Transform> _destinations;
        private Vector3 _currentDestination;
        private SpriteRenderer _spriteRenderer;
        private Animator _animator;
        private SoundSystem _soundSystem;
        private Coroutine _wormMoveRoutine;

        public static event Action OnWormPickedUp;
        public static event Action OnWormDroped;
   

        // set worm to some point on scene and monitor whether it has been picked
        public void Construct(Vector2 position, InputSystem inputSystem, SoundSystem soundSystem, List<Transform> destinations)
        {
            _soundSystem = soundSystem;
            transform.position = position;
            _destinations = destinations;
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _dragAndDrop.Construct(inputSystem);
            _dragAndDrop.OnDragStart += TakeWorm;
            _dragAndDrop.OnDragEnded += PutWorm;

            MoveToRandomPoint();
        }

        private IEnumerator PlayCrowling()
        {
            while(true)
            {
                _soundSystem.PlaySound(crawlingSound);
                yield return new WaitForSeconds(1f);
            }
        }
        // set random direction of worm's movement 
        private void MoveToRandomPoint()
        {
            if(_wormMoveRoutine == null)            
                _wormMoveRoutine ??= StartCoroutine(PlayCrowling());
            _currentDestination = GetRandomPositionToMove();
            var timeToMove = Vector2.Distance(transform.position, _currentDestination) / _wormSpeed;
            Flip(_currentDestination);
            transform.DOMove(_currentDestination, timeToMove).onComplete += MoveToRandomPoint;
        }
        
        // used when worm's position is on left or right side of screen to flip sprite in right direction
        private void Flip(Vector3 destination)
        {
            if ((transform.position - destination).x > 0)
                _spriteRenderer.flipX = true;
            else
                _spriteRenderer.flipX = false;
        }

        // choose random direction of movement
        private Vector3 GetRandomPositionToMove()
        {
            var randPosition = _destinations[Random.Range(0, _destinations.Count)].position;
            while (transform.position == randPosition)
            {
                randPosition = _destinations[Random.Range(0, _destinations.Count)].position;
            }
        
            return randPosition;
        }
    
        // change animation to "OscillationsOfWorm" and invoke [OnWormPickedUp]
        private void TakeWorm()
        {
            
            StopCoroutine(_wormMoveRoutine);
            _wormMoveRoutine = null;
            
            OnWormPickedUp?.Invoke();
            transform.DOKill();
            _spriteRenderer.flipY = false;
            _animator.SetBool("IsTaken",true);
        }

        // worm moves to ground also change animation to "WormMoveAnimation" and invoke [OnWormDroped]
        // make worm unavailable for dragging until it moves to ground
        private void PutWorm()
        {
            _collider.enabled = false;
            _dragAndDrop.IsDraggable = false;
            OnWormDroped?.Invoke();
            _animator.SetBool("IsTaken", false);
            MoveToGround();
        }
    
        // after worm moves to "ground" it becomes available for dragging
        public void MoveToGround()
        {
            var destination = new Vector2(transform.position.x, _currentDestination.y);
            transform.DOMove(destination, 0.5f)
                .OnComplete(() => 
                {
                    MoveToRandomPoint();   
                    _dragAndDrop.IsDraggable = true;
                    _collider.enabled = true;
                });
        }

        // worm moves to chicken's jaw and disappears
        public void Disappearing(Vector2 destination)
        {
            OnWormDroped?.Invoke();
            _dragAndDrop.IsDraggable = false;
            transform.DOMove(destination, 0.5f);
            transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
            {
                OnDestroy();
            });
        }

        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= TakeWorm;
            _dragAndDrop.OnDragEnded -= PutWorm;
        }
    }
}
