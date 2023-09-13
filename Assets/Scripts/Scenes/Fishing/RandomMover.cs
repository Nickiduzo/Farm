using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Fishing
{
    public class RandomMover : MonoBehaviour
    {
        private const string SpeedKey = "Speed";

        [SerializeField] private bool _rotateInsteadOfFlip = false;
        [SerializeField] private bool _canMoveVertically = false;
        [SerializeField] private bool _canIdle = false;
        [SerializeField] private float _maxMoveSpeed = 4;
        [SerializeField] private float _minMoveSpeed = 2;
        [SerializeField] private float _distance = 10f;
        [SerializeField] private float _screenOffset;
        [SerializeField] private List<SpriteRenderer> _sprites;
        [SerializeField] private float _accelerationValue;

        private float _moveSpeed;
        private float _maxDistance;
        private float _minDistance;
        private float initialYPos;
        private float idleMinDur = 1;
        private float idleMaxDur = 10;
        private float _verticalSpeed = 0.25f;
        
        private bool IsCanMove => _canIdle && !IsIdling || !_canIdle;
        private bool IsIdling { get; set; }
       
        private Animator Animator { get; set; }

        private bool StopOnDestination { get; set; } = false;
        
        private float VerticalSpeed
        {
            get
            {
                CheckVerticalSpeedDirection();
                return _verticalSpeed;
            }
            set
            {
                _verticalSpeed = value;
            }
        }
        
        /// <summary>
        /// Викликає ф-цію "Init"
        /// </summary>
        private void Awake()
            => Init();

        /// <summary>
        /// Викликає ф-цію "Move"
        /// </summary>
        private void Update()
            => Move();
        
        /// <summary>
        /// Змінює значення поля "_verticalSpeed" на протилежне
        /// </summary>
        private void CheckVerticalSpeedDirection()
        {
            if (this.transform.position.y >= initialYPos + this.transform.localScale.z && _verticalSpeed > 0)
                _verticalSpeed *= -1;
            if (this.transform.position.y <= initialYPos && _verticalSpeed < 0)
                _verticalSpeed *= -1;
        }

        /// <summary>
        /// Вводимо аніматор [animator] -
        /// запам'ятовуємо в властивості "Animator" значення параметру
        /// </summary>        
        public void Construct(Animator animator = null)
        {
            Animator = animator;
        }

        /// <summary>
        /// запам'товуємо в властивості "VerticalSpeed" значення поля "_verticalSpeed",
        /// позицію елементу в полі "initialYPos",
        /// запам'ятовуємо значення ф-ції "GetRandomMoveSpeed" в полі "_moveSpeed"
        /// 
        /// </summary>
        private void Init()
        {
            VerticalSpeed = _verticalSpeed;
            initialYPos = this.transform.position.y;
            _moveSpeed = GetRandomMoveSpeed();
            CalculateDistance();
        }

        /// <summary>
        /// Рухає елемент в напрямку поля "movement", виконує ф-ції "Flip", "CalculateAcceleration"
        /// </summary>
        private void Move()
        {
            if (!IsCanMove) return;

            Vector3 verticalMovement = _canMoveVertically ? transform.up * (VerticalSpeed * Time.deltaTime) : Vector3.zero;
            Vector3 movement = transform.right * (_moveSpeed * Time.deltaTime) + verticalMovement;
            transform.Translate(movement);
            Flip();
            CalculateAcceleration();
        }

        /// <summary>
        /// Рахує прискорення та додає полю "_moveSpeed"
        /// </summary>
        private void CalculateAcceleration()
            => _moveSpeed += _moveSpeed < 0 ? -_accelerationValue : _accelerationValue;

        /// <summary>
        /// Рахує мінімальну [_minDistance] та максимальну дистанції переміщення [_maxDistance]
        /// </summary>
        private void CalculateDistance()
        {
            if (Camera.main == null) return;

            Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            _maxDistance = Mathf.Clamp(transform.position.x + _distance, -screenBounds.x, screenBounds.x + _screenOffset);
            _minDistance = Mathf.Clamp(transform.position.x - _distance, -(screenBounds.x + _screenOffset), screenBounds.x + _screenOffset);
        }

        /// <summary>
        /// Викликає зупинку елементу з тривалістю "Random.Range(idleMinDur, idleMaxDur)"
        /// </summary>
        private void Idle()
        {
            var Sequence = DOTween.Sequence();
            if (_canIdle)
            {
                Sequence.AppendCallback(() => IsIdling = true);
                Sequence.AppendInterval(Random.Range(idleMinDur, idleMaxDur));
                Sequence.AppendCallback(() => IsIdling = false);
            }
        }

        /// <summary>
        /// Повертає елемент, змінює на протилежні значення полів "_moveSpeed", "_verticalSpeed" 
        /// і викликає ф-цію "Idle"
        /// </summary>
        private void Flip()
        {
            if (StopOnDestination)
                _moveSpeed = _moveSpeed > 0 ? _maxMoveSpeed * 2 : -_maxMoveSpeed * 2;

            if (transform.position.x >= _maxDistance)
            {
                if (_sprites.Count == 0)
                {
                    float XScale = transform.localScale.x > 0 ? -transform.localScale.x : transform.localScale.x;
                    transform.localScale = new Vector3(XScale, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    if (_rotateInsteadOfFlip)
                    {
                        this.transform.rotation = Quaternion.Euler(Vector3.zero);
                    }
                    else
                    {
                        foreach (SpriteRenderer sprite in _sprites)
                            sprite.flipX = false;
                    }
                }
                _moveSpeed = GetRandomMoveSpeed();
                _verticalSpeed *= -1;
                Idle();
            }

            if (transform.position.x <= _minDistance)
            {
                if (_sprites.Count == 0)
                {
                    float XScale = transform.localScale.x < 0 ? -transform.localScale.x : transform.localScale.x;
                    transform.localScale = new Vector3(XScale, transform.localScale.y, transform.localScale.z);
                }
                else
                {
                    if (_rotateInsteadOfFlip)
                    {
                        this.transform.rotation = Quaternion.Euler(0, 180, 0);
                    }
                    else
                    {
                        foreach (SpriteRenderer sprite in _sprites)
                            sprite.flipX = true;
                    }
                }
                _moveSpeed = GetRandomMoveSpeed() * -1;
                _verticalSpeed *= -1;
                Idle();
            }

        }

        /// <summary>
        /// Повертає швидкість із значенням "-Random.Range(_minMoveSpeed, _maxMoveSpeed)", 
        /// якщо значення "StopOnDestination" дорівнює "false"
        /// </summary>        
        private float GetRandomMoveSpeed()
        {
            var speed = StopOnDestination ? 0 : -Random.Range(_minMoveSpeed, _maxMoveSpeed);
            if (Animator) Animator.SetFloat(SpeedKey, Mathf.Abs(speed));
            return speed;
        }

        /// <summary>
        /// Зупиняє елемент при досягненні точки призначення
        /// </summary>
        internal void StopOnReachingDestination()
        {
            _accelerationValue = 0;
            Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
            _maxDistance = screenBounds.x + _screenOffset;
            _minDistance = -screenBounds.x - _screenOffset;
            StopOnDestination = true;
        }
    }
}