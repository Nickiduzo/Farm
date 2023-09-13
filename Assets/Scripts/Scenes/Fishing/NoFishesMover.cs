using UnityEngine;
using DG.Tweening;
using AwesomeTools.Sound;

namespace Fishing
{
    public class NoFishesMover : MonoBehaviour
    {   
        private const string CaughtSoundCrab = "CaughtCrab";
        private const string CaughtSoundOctopus = "CaughtOctopus";
        private const string IdleAnimationName = "IdleCrab";
        private const string MoveAnimationName = "MoveCrab";

        [SerializeField] private Animator _animator;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private Transform[] _waypoints;
        [SerializeField] private Transform _mainWaypointCrab;
        [SerializeField] private Transform _mainWaypointOctopus;
        [SerializeField] private float _maxMoveSpeed = 3.5f;
        [SerializeField] private float _minMoveSpeed = 1.5f;
        [SerializeField] private float _minDelay = 2f;
        [SerializeField] private float _maxDelay = 6f;

        private int _currentWaypointIndex;
        private bool _isWaitingAtWaypoint;
        private bool _isFacingLeft;
        private bool _isMovementPaused;
        private bool _canMoveToNexttWaypoint = true;
        private float _currentDelay;

        private Tween _movementTween;
        private Tween _delayTween;

        // set "transform.position" in random [_waypoints], start wait delay
        private void Start()
        {
            if (_waypoints.Length > 0)
            {
                _currentWaypointIndex = GetRandomWaypointIndex();
                transform.position = _waypoints[_currentWaypointIndex].position;
                _currentDelay = GetRandomDelay();
                _isWaitingAtWaypoint = true;
                MoveToNextWaypoint();
            }
        }

        // choose random "Waypoint", stop any movement, if it was and after delay launch "MoveToNextWaypoint" again 
        private void MoveToNextWaypoint()
        {
            _currentWaypointIndex = GetRandomWaypointIndex();
            _currentDelay = GetRandomDelay();
            _isWaitingAtWaypoint = true;

            OnDestroy();

            _delayTween = DOTween.Sequence()
                .AppendInterval(_currentDelay)
                .OnComplete(() =>
                {
                    _isWaitingAtWaypoint = false;
                    if(_canMoveToNexttWaypoint)
                    {
                        MoveToNextWaypoint();
                    }
                });

            Vector3 targetPosition = _waypoints[_currentWaypointIndex].position;
            float moveDuration = Vector3.Distance(transform.position, targetPosition) / GetRandomMoveSpeed();

            _movementTween = transform.DOMove(targetPosition, moveDuration)
                .SetEase(Ease.InOutQuad)
                .OnStart(() =>
                {
                    _isWaitingAtWaypoint = true;
                    PlayAnimation(true);
                    FlipIfNeeded(targetPosition);
                })
                .OnComplete(() =>
                {
                    _isWaitingAtWaypoint = true;
                    FlipIfNeeded(targetPosition);
                    PlayAnimation(false);
                });
        }

        // check whether needed flip, if so, flip to right side
        private void FlipIfNeeded(Vector3 targetPosition)
        {
            if (_currentWaypointIndex >= 0 && _currentWaypointIndex < _waypoints.Length)
            {
                Vector3 nextWaypointPosition = _waypoints[_currentWaypointIndex].position;
                if (nextWaypointPosition.x < targetPosition.x)
                {
                    Flip(false);
                }
                else
                {
                    Flip(true);
                }
            }
        }

        // flip gameobject based on [isLeft]
        private void Flip(bool isLeft)
        {
            if (isLeft && !_isFacingLeft || !isLeft && _isFacingLeft)
            {
                _isFacingLeft = isLeft;
                Vector3 scale = transform.localScale;
                scale.x *= -1;
                transform.localScale = scale;
            }
        }

        // get new [randomIndex] and return it, also avoid the same index
        private int GetRandomWaypointIndex()
        {
            int randomIndex = Random.Range(0, _waypoints.Length);
            while (randomIndex == _currentWaypointIndex)
            {
                randomIndex = Random.Range(0, _waypoints.Length);
            }

            return randomIndex;
        }

        // get random "speed" and retur it
        private float GetRandomMoveSpeed()
        {
            var speed = Random.Range(_minMoveSpeed, _maxMoveSpeed);
            return speed;
        }
        
        // uses when object is caught, stop all tweens (movements)
        public void PauseMovement()
        {
            if(gameObject.name == "Crab")
                _soundSystem.PlaySound(CaughtSoundCrab);
            else
                _soundSystem.PlaySound(CaughtSoundOctopus);

            if (!_isMovementPaused)
            {
                _isMovementPaused = true;

                _movementTween?.Pause();
                _delayTween?.Pause();

                PlayAnimation(false);
            }
        }

        // uses when object was put, resume all tweens 
        public void ResumeMovement()
        {
            if (_isMovementPaused)
            {
                _isMovementPaused = false;

                _movementTween?.Play();
                _delayTween?.Play();

                PlayAnimation(true);
            }
        }

        // uses fro stop all tweens
        private void OnDestroy()
        {
            if (_movementTween != null)
            {
                _movementTween.Kill();
                _movementTween = null;
            }

            if (_delayTween != null)
            {
                _delayTween.Kill();
                _delayTween = null;
            }
        }

        // Animation controller for switching aniamtions based on [CanMove]
        private void PlayAnimation(bool CanMove)
        {            
            if(CanMove == true)
            {
                _animator.SetBool("CanMove", CanMove);
            }
            else
            {
                _animator.SetBool("CanMove", CanMove);
            }
        }
        
        // Move Octopus/Crab to left and right edges of camera
        public void MoveToEdges()
        {
            PlayAnimation(false);
            _animator.CrossFade("Idle", 2);
            transform.DOKill();
            _canMoveToNexttWaypoint = false;
            
            if(_mainWaypointCrab == null)
            {
                transform.DOMove(_mainWaypointOctopus.position, 4);
            }

            if(_mainWaypointOctopus == null)
            {
                transform.DOMove(_mainWaypointCrab.position, 4);
            }
        }

        // get random delay and return it
        private float GetRandomDelay()
        {
            return Random.Range(_minDelay, _maxDelay);
        }
    }
}
