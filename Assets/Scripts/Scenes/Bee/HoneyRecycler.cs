using Bee.Config;
using Bee.Triggers;
using DG.Tweening;
using System;
using UnityEngine;
using UsefulComponents;

namespace Bee
{
    public class HoneyRecycler : MonoBehaviour
    {
        public event Action OnAllHoneyRecycled;
        [SerializeField] private ParticleSystem _catchFx;
        [SerializeField] private BeeLevelConfig _config;
        [SerializeField] private HoneyGrinder _honeyGrinder;
        [SerializeField] private MoveStartDestination _move;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private Transform _storePosition;
        [SerializeField] private Transform _preStorePosition;
        [SerializeField] private Transform _hintTransform;
        [SerializeField] private HoneyTriggerObserver _trigger;

        public event Action StartRecycling;
        public event Action EndRecycling;

        private int _honeyRecycledCount;
        public Vector3 HintPosition => _hintTransform.position;

        // It subscribes from events
        private void Awake()
        {
            _trigger.OnTriggerEnter += MoveToPreStorePosition;
            _honeyGrinder.OnGrind += Recycled;
        }
        // It unsubscribes from events
        private void OnDestroy()
            => _trigger.OnTriggerEnter -= MoveToPreStorePosition;

        // Move the object to the destination
        public void MoveToDestination()
        {
            MakeNonInteractable();
            _move.MoveToDestination()
                .OnComplete(MakeInteractable);
        }

        // Process the honeycomb
        private void ProcessHoneyComb(HoneyComb honey)
        {
            StartRecycling?.Invoke();
            ShowCatchFx(honey.gameObject.transform.position);
            MakeNonInteractable();
            honey.SetVisualInteractOrder();
            honey.transform.SetParent(transform);
            MoveToStorePosition(honey);
            _honeyGrinder.ProcessHoney(honey);
            HintSystem.Instance.HidePointerHint();
        }

        // Triggered when honey is recycled
        private void Recycled()
        {
            EndRecycling?.Invoke();
            ShowCatchFx(transform.position);
            _honeyRecycledCount++;

            if (IsEnoughRecycled())
            {
                OnAllHoneyRecycled?.Invoke();
            }
            else
            {
                MakeInteractable();
            }
        }

        // Check if enough honey is recycled
        private bool IsEnoughRecycled()
        {
            return _honeyRecycledCount >= _config.Hives.Length;
        }

        // Move the honeycomb to the pre-store position
        private void MoveToPreStorePosition(HoneyComb honey)
        {
            if (!honey._availableForRecycler)
            {
                honey.Stored(_preStorePosition);
                honey.transform.DOMove(_preStorePosition.position, 1f)
                    .OnComplete(() =>
                    {
                        ProcessHoneyComb(honey);
                    });
            }
        }

        // Move the honeycomb to the store position
        private void MoveToStorePosition(HoneyComb honey)
        {
            honey.transform.DOMove(_storePosition.position, 1f);
        }

        // Make the object non-interactable
        private void MakeNonInteractable()
        {
            _collider.enabled = false;
        }

        // Make the object interactable
        private void MakeInteractable()
        {
            _collider.enabled = true;
        }

        // Show the catch particle effect
        private void ShowCatchFx(Vector3 target)
        {
            Instantiate(_catchFx, target, Quaternion.identity);
        }

    }
}