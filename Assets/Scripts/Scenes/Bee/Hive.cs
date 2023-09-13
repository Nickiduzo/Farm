using AwesomeTools.Sound;
using System;
using UnityEngine;
using AwesomeTools;
using UsefulComponents;

namespace Bee
{
    public class Hive : MonoBehaviour, IHive
    {
        private const string CATCH = "Catch";
        private const string SUCCESS = "Success";
        public event Action OnOpened;
        public event Action OnStored;

        [SerializeField] private ParticleSystem _openFx;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private HoneyComb _honeyComb;
        [SerializeField] private Transform _roofDestinationPosition;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private MoveStartDestination _hiveRoofMover;
        [SerializeField] private Animation _animation;
        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;

        public IHoneyComb HoneyComb => _honeyComb;
        public bool IsOpened { get; private set; }

        public Vector3 StayPosition { get; private set; }
        // Perform construction
        public void Construct(ISoundSystem soundSystem, FxSystem fxSystem)
        {
            StayPosition = transform.position;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
            _honeyComb.Construct(soundSystem, fxSystem);
        }

        // Perform initialization
        public void Init()
        {
            _hiveRoofMover.Construct(_roofDestinationPosition.position, _hiveRoofMover.transform.position);
            _mouseTrigger.OnDown += Open;
            _honeyComb.OnStored += Close;
            _animation.Play();
        }

        // Clean up event subscriptions
        private void OnDestroy()
        {
            _mouseTrigger.OnDown -= Open;
            _honeyComb.OnStored -= Close;
        }

        // Open the hive
        private void Open()
        {
            if (IsOpened) return;
            _soundSystem.PlaySound(CATCH);
            _fxSystem.PlayEffect(SUCCESS, transform.position);
            IsOpened = true;

            _animation.clip.SampleAnimation(gameObject, 0);
            _animation.Stop();
            OpenRoof();
            OnOpened?.Invoke();
            _collider.enabled = false;
        }
        // Close the hive
        private void Close()
        {
            OnStored?.Invoke();
            CloseRoof();
            IsOpened = false;
        }

        // Open the hive roof
        private void OpenRoof()
            => _hiveRoofMover.MoveToDestination();

        // Close the hive roof
        private void CloseRoof()
            => _hiveRoofMover.MoveToStart();

    }
}