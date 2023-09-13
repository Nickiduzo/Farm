using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using System.Collections;
using AwesomeTools;

namespace Bee
{
    public class HoneyGrinder : MonoBehaviour
    {
        private const string GRIND = "Grind";
        public event Action OnGrind;

        [SerializeField] private Collider2D _collider;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private GrinderMechanic _grinderMechanic;
        [SerializeField] private SpriteRenderer _rotateRenderer;
        [SerializeField] private Sprite[] _rotateSprites;

        private IHoneyComb _honeyComb;
        private InputSystem _inputSystem;

        private bool isFirstHoney;
        private bool _isRotatingSprite;
        private Vector2 _prevMousePos;
        private ISoundSystem _soundSystem;

        private float _idleTime = 0f;
        private const float IDLE_THRESHOLD = .2f;

        // It init components
        public void Construct(ISoundSystem soundSystem, InputSystem inputSystem)
        {
            _soundSystem = soundSystem;
            _inputSystem = inputSystem;
            _grinderMechanic.Init(_soundSystem,GRIND);
        }

        // It subscribes from events
        private void Awake()
        {
            isFirstHoney = true;

            MakeNonInteractable();
            _mouseTrigger.OnDrag += CheckDrag;
            _mouseTrigger.OnUp += _grinderMechanic.StopGrinding;
            _mouseTrigger.OnUp += StopChangingRotateSprite;
            _grinderMechanic.Init();
            _grinderMechanic.progressCalculator.OnProgressStep += HoneyGrinded;
            _grinderMechanic.GrindingRoutineAction += MoveDownPerStep;
        }

        // Move the honeycomb down per step
        private void MoveDownPerStep()
        {
            _honeyComb.MoveDownPerStep();
        }

        // Check if the honeycomb is being dragged
        private void CheckDrag()
        {
            var newMousePos = _inputSystem.CalculateTouchPosition();

            var collider = Physics2D.OverlapPoint(newMousePos);

            if (!collider || collider.gameObject != gameObject)
            {
                StopChangingRotateSprite();
                _grinderMechanic.StopGrinding();
                return;
            }

            if (_prevMousePos != Vector2.zero)
            {
                var toNewPos = newMousePos - (Vector2)transform.position;
                var toPrevPos = _prevMousePos - (Vector2)transform.position;
                var angle = Vector2.SignedAngle(toPrevPos, toNewPos);
                var speed = toNewPos.magnitude / Time.deltaTime;

                if (angle < 0f && speed > 1f)
                {
                    _idleTime = 0f;
                    _grinderMechanic.StartGrinding();
                    StartChangingRotateSprite();
                }
                else
                {
                    _idleTime += Time.deltaTime;

                    if (_idleTime > IDLE_THRESHOLD)
                    {
                        _grinderMechanic.StopGrinding();
                    }
                }
            }

            _prevMousePos = newMousePos;
        }

        // launch "RotateSpriteCoroutine"
        private void StartChangingRotateSprite()
        {
            if (!_isRotatingSprite)
            {
                _isRotatingSprite = true;
                StartCoroutine(RotateSpriteCoroutine());
            }
        }

        // stop all coroutines
        private void StopChangingRotateSprite()
        {
            if (_isRotatingSprite)
            {
                _isRotatingSprite = false;
                StopAllCoroutines();
            }
        }

        // changing rotate spriteRenderer and flip it if needed
        private IEnumerator RotateSpriteCoroutine()
        {
            int spriteIndex = 0;
            bool isFlipping = false;

            while (_isRotatingSprite)
            {
                if (spriteIndex == 1)
                {
                    _rotateRenderer.flipY = true;
                    isFlipping = true;
                }
                else if (spriteIndex != 2 && isFlipping)
                {
                    _rotateRenderer.flipY = false;
                    isFlipping = false;
                }

                _rotateRenderer.sprite = _rotateSprites[spriteIndex];

                spriteIndex = (spriteIndex + 1) % _rotateSprites.Length;

                yield return new WaitForSeconds(0.3f);
            }

            _rotateRenderer.flipY = false;
        }


        // Called when honey is grinded
        private void HoneyGrinded()
        {
            _soundSystem.PlaySound(GRIND);
            _honeyComb.OnRecycled();
            MakeNonInteractable();
            _grinderMechanic.StopGrinding();
            OnGrind?.Invoke();
            _grinderMechanic._appearAndDisappear.Disappear();
        }

        // Cleanup when the object is destroyed
        private void OnDestroy()
        {
            _mouseTrigger.OnUp -= _grinderMechanic.StopGrinding;
            _mouseTrigger.OnDrag -= CheckDrag;
            _grinderMechanic.progressCalculator.OnProgressStep -= HoneyGrinded;
        }

        // Process the honeycomb
        public void ProcessHoney(IHoneyComb honeyComb)
        {
            if (isFirstHoney)
            {
                isFirstHoney = false;
            }

            _grinderMechanic._appearAndDisappear.Appear();

            _honeyComb = honeyComb;
            MakeInteractable();
        }

        // Make the Honey interactable
        private void MakeInteractable()
        {
            _collider.enabled = true;
        }

        // Make the Honey non-interactable
        private void MakeNonInteractable()
        {
            _collider.enabled = false;
        }
    }
}