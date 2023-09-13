using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

namespace Tomato
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
        [SerializeField] private float _maxYDistance;

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
        /// При завантаженні скрипту викликає метод Init()
        /// </summary>
        private void Awake()
            => Init();

       /// <summary>
       /// Кожний кадр викликає метод Move() 
       /// </summary>
        private void Update()
            => Move();

      /// <summary>
      /// Змінює напрямок вертикальної швидкості [_verticalSpeed] 
      /// </summary>
        private void CheckVerticalSpeedDirection()
        {
            if (this.transform.position.y >= initialYPos + _maxYDistance && _verticalSpeed > 0)
                _verticalSpeed *= -1;
            if (this.transform.position.y <= initialYPos && _verticalSpeed < 0)
                _verticalSpeed *= -1;
        }

        /// <summary>
        /// Вводимо мінімальну [minDistance] та максимальну дистанції [maxDistance] - присвоює мінімальну [_minDistance] та максимальну швидкості [_maxDistance]    
        /// </summary>
        /// <param name="minDistance">мінімальна дистанція</param>
        /// <param name="maxDistance">максимальна дистанція</param>
        public void Construct(float minDistance, float maxDistance)
        {
            _minDistance = minDistance;
            _maxDistance = maxDistance;
        }

        /// <summary>
        /// Присвоює приватну вертикальну швидкість [_verticalSpeed] до публічної вертикальної швидкості [VerticalSpeed].
        /// Виконує ф-ції InitialPosY() та CalculateDistance().
        /// Присвоює значенния ф-ції GetRandomMoveSpeed() до швидкості руху елементу[_moveSpeed]
        /// </summary>
        private void Init()
        {
            VerticalSpeed = _verticalSpeed;
            InitialPosY();
            _moveSpeed = GetRandomMoveSpeed();
            CalculateDistance();
        }

    /// <summary>
    /// Присвоює полю з початковим значенням "position.y" [initialYPos] значення елементу [transform.position.y]  
    /// </summary>
        public void InitialPosY()
        {
            initialYPos = transform.position.y;
        }
        
        /// <summary>
        /// Рахує значення вертикальної швидкості [verticalMovement] і переміщення [movement],
        /// рухає елемент, виконує ф-ції Flip(), CalculateAcceleration()
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
        /// Додає до швидкості руху [_moveSpeed] значення прискорення [_accelerationValue]
        /// </summary>
        private void CalculateAcceleration()
            => _moveSpeed += _moveSpeed < 0 ? -_accelerationValue : _accelerationValue;

       /// <summary>
       /// Присвоює максимальну [_maxDistance] та мінінмальну дистанції [_minDistance] для елементу 
       /// </summary>
        private void CalculateDistance()
        {
            if (Camera.main == null) return;

            Vector3 screenBounds = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));

            _maxDistance = Mathf.Clamp(transform.position.x + _distance, -screenBounds.x - _screenOffset, screenBounds.x + _screenOffset);
            _minDistance = Mathf.Clamp(transform.position.x - _distance, -screenBounds.x - _screenOffset, screenBounds.x + _screenOffset);

        }

        /// <summary>
        /// Зупиняє рух елементу
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
        /// Повертає елемент після досягнення мінімальної [_minDistance] або максимальної дистанції [_maxDistance]
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
                            sprite.flipX = !sprite.flipX;
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
                            sprite.flipX = !sprite.flipX;
                    }
                }
                _moveSpeed = GetRandomMoveSpeed() * -1;
                _verticalSpeed *= -1;
                Idle();
            }

        }

        /// <summary>
        ///  Повертає випадкову швидкість руху
        /// </summary>
        /// <returns>Швидкість</returns>
        private float GetRandomMoveSpeed()
        {
            var speed = StopOnDestination ? 0 : -Random.Range(_minMoveSpeed, _maxMoveSpeed);
            if (Animator) Animator.SetFloat(SpeedKey, Mathf.Abs(speed));
            return speed;
        }
    }
}