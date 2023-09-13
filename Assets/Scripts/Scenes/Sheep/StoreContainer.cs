using System;
using Bee;
using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Sheep
{
    public class StoreContainer : MonoBehaviour
    {
        private const string COMPRESS_STRING = "Compress";
        private const float IDLE_THRESHOLD = 0.8f;
        private const int FUR_SCRATCH_COUNT_BY_SHEEP = 5;

        public event Action OnComposedFurReady;
        public bool IsDraggable;

        [SerializeField] private SheepLevelConfig _config;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private GameObject _furContainerObj;
        [SerializeField] private GrinderMechanic _grinderMechanic;

        private int _compressFurCount;
        private InputSystem _inputSystem;
        private SoundSystem _soundSystem;

        private float _idleTime = 0f;
        private Vector2 _prevMousePos;
        private bool _isPointerHintActive = true;

        //Constructs Store container
        public void Construct(SoundSystem sound, InputSystem inputSystem)
        {
            _grinderMechanic.Init();
            _grinderMechanic.Init(sound, COMPRESS_STRING);
            _soundSystem = sound;
            _inputSystem = inputSystem;
            _mouseTrigger.OnDrag += Grind;
            _mouseTrigger.OnUp += _grinderMechanic.StopGrinding;
            _grinderMechanic.progressCalculator.OnProgressStep += FurComposed;
        }

        private void OnDestroy()
        {
            _mouseTrigger.OnDrag -= Grind;
            _mouseTrigger.OnUp -= _grinderMechanic.StopGrinding;
            if(_grinderMechanic.progressCalculator != null)
                _grinderMechanic.progressCalculator.OnProgressStep -= FurComposed;
        }

        // Grind the fur
        private void Grind()
        {
            IsMouseMoveAroundCircle();

            if (_isPointerHintActive)
            {
                _isPointerHintActive = false;
                HintSystem.Instance.HidePointerHint();
                StopCoroutine(Shop.PointerHintRoutine);
                Shop.PointerHintRoutine = null;
            }
        }

        // Called when the fur is composed
        private void FurComposed()
        {
            _compressFurCount++;

            // Destroy the fur scratch game objects
            for (var i = 0; i < FUR_SCRATCH_COUNT_BY_SHEEP; i++)
            {
                Destroy(_furContainerObj.transform.GetChild(i).gameObject);
            }

            OnComposedFurReady?.Invoke();

            DisappearWithScale();
        }

        // Check if the mouse is moving around the circle
        private void IsMouseMoveAroundCircle()
        {
            var newMousePos = _inputSystem.CalculateTouchPosition();

            if (_prevMousePos != Vector2.zero)
            {
                var toNewPos = newMousePos - (Vector2)transform.position;
                var toPrevPos = _prevMousePos - (Vector2)transform.position;
                var angle = Vector2.SignedAngle(toPrevPos, toNewPos);

                if (angle < 0f)
                {
                    _idleTime = 0f;
                    _grinderMechanic.StartGrinding();
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

        // Disappear the fur with scale animation
        private void DisappearWithScale()
        {
            if (!IsCompressEnough())
            {
                return;
            }

            IsDraggable = false;
            _soundSystem.StopSound(COMPRESS_STRING);
            _mouseTrigger.enabled = false;
            _grinderMechanic.rotateHint.Disappear();
            Disappear();
        }

        // Check if enough fur has been compressed
        private bool IsCompressEnough()
            => _compressFurCount >= _config.ComposedFurToSpawn;

        // Disappear the fur with scale animation
        private void Disappear()
        {
            transform.DOScale(Vector3.zero, 0.5f);
        }

    }
}