using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Bee
{
    public class HoneyTap : MonoBehaviour, IHoneyTap
    {
        private const string HoneyStream = "HoneyStream";
        public event Action OnHoneyStreamEnabled;

        [SerializeField] private Transform _tapKnob;
        [SerializeField] private Transform _honeyStream;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private Animation _animation;
        [Header("Sprites")]
        [SerializeField] private SpriteRenderer _backPartOfRecycler;
        [SerializeField] private SpriteRenderer _rotate;
        [SerializeField] private SpriteRenderer[] _spritesKrana;

        private ISoundSystem _soundSystem;
        private int _sortingOrderBackPart;
        
        private bool _isTapped;
        // It init systems and subscribe to event 
        public void Construct(ISoundSystem soundSystem)
        {
            _sortingOrderBackPart = _backPartOfRecycler.sortingOrder;
            _soundSystem = soundSystem;
        }
        // It subscribes from events
        private void Awake()
            => _mouseTrigger.OnDown += EnableHoneyStream;
        // It unsubscribes from events
        private void OnDestroy()
            => _mouseTrigger.OnDown -= EnableHoneyStream;

        // Make the object interactable
        public void MakeInteractable()
        {
            _animation.Play();
            _collider.enabled = true;
        }

        // Make the object non-interactable
        public void MakeNonInteractable()
        {
            _collider.enabled = false;
        }

        // Set a new sorting order for sprites
        public void SetNewSortingOrder()
        {
            for (var i = 0; i < _spritesKrana.Length; i++)
            {
                _spritesKrana[i].sortingOrder -= 8;
                _backPartOfRecycler.sortingOrder = _sortingOrderBackPart - 2;
                _rotate.sortingOrder = _sortingOrderBackPart - 2;
            }
        }

        // Enable the honey stream
        private void EnableHoneyStream()
        {
            if (_isTapped)
            {
                return;
            }

            _isTapped = true;
            _animation.Stop();
            HintSystem.Instance.HidePointerHint();

            RotateKnob(35f).OnComplete(() =>
            {
                OnHoneyStreamEnabled.Invoke();

                Sequence sequence = DOTween.Sequence();
                sequence.Append(_honeyStream.DOScaleY(1.2f, 0.4f));
                sequence.Append(_honeyStream.DOScaleY(0.68f, 1.7f));
                sequence.Play();

                _soundSystem.PlaySound(HoneyStream);
            });
        }

        // Disable the honey stream
        public Tween DisableHoneyStream()
        {
            RotateKnobBack(0).OnComplete(() =>
            {
                _isTapped = false;
            });
            _soundSystem.StopSound(HoneyStream);

            Sequence sequence = DOTween.Sequence();
            sequence.Join(_honeyStream.DOScaleY(0, 0.5f));
            sequence.Join(_honeyStream.DOScaleX(0.1f, 0.5f));
            sequence.Join(_honeyStream.DOLocalMoveY(-2.5f, 0.5f));
            sequence.Append(_honeyStream.DOLocalMoveY(-1.45f, 0.01f));
            sequence.Join(_honeyStream.DOScaleX(1f, 0.01f));
            sequence.Play();
            return sequence;
        }

        // Rotate the knob
        private Tween RotateKnob(float angle)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Join(_tapKnob.DORotate(new Vector3(0, 0, angle), 1f));
            sequence.Join(_tapKnob.DOLocalMoveX(0.64f, 1f));
            sequence.Join(_tapKnob.DOLocalMoveY(0.35f, 1f));
            return sequence;
        }

        private Tween RotateKnobBack(float angle)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.Join(_tapKnob.DORotate(new Vector3(0, 0, angle), 1f));
            sequence.Join(_tapKnob.DOLocalMoveX(0.41f, 1f));
            sequence.Join(_tapKnob.DOLocalMoveY(0.49f, 1f));
            return sequence;
        }

    }
}