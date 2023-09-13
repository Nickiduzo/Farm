using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools;
using AwesomeTools.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CowScene
{
    public class CowMilkShop : MonoBehaviour
    {

        private const string SUCCESS = "Success";

        public event Action ShopArrived;
        public event Action<Jar> OnJarSpill;
        public event Action BottleMoved;

        [Header("Observer")]
        [SerializeField] private MilkJarTriggerObserver _jarTrigger;
        [Header("Positions")]
        [SerializeField] private Transform _filledBottlePosition;
        [SerializeField] private Transform _jarSpillingPosition;
        [SerializeField] private Transform _spillFxPosition;
        [Header("Misc")]
        [SerializeField] private List<Jar> _jars;
        [SerializeField] private InputSystem _inputSystem;

        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;

        private Vector3 _destination;
        private float _bottleStep;

        public List<Jar> Jars => _jars;

        // It init MilkShop
        public void Construct(Vector3 destination, Vector3 end, ISoundSystem soundSystem, FxSystem fxSystem, InputSystem inputSystem)
        {
            _inputSystem = inputSystem;
            _destination = destination;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;

            _jarTrigger.OnTriggerEnter += OnSpillTrigger;

            MoveToDestination().OnComplete(InvokeOnArrived);
        }

        // Move to the destination point
        public Tween MoveToDestination()
        {
            return MoveTo(_destination);
        }

        // Move to a specific target point
        private Tween MoveTo(Vector3 targetPoint)
        {
            return transform.DOMove(targetPoint, 1f);
        }

        // Construct the jars and set up their interactions
        private void ConstructJars()
        {
            foreach (var jar in _jars)
            {
                jar.Construct(jar.transform.position, jar.transform.position, _soundSystem);
                jar.GetComponent<DragAndDrop>().Construct(_inputSystem);
            }
        }

        // Triggered when the spill trigger is activated
        private void OnSpillTrigger(Jar jar)
        {
            _jarTrigger.OnTriggerEnter -= OnSpillTrigger;

            foreach (var _jar in _jars)
            {
                _jar.MakeNonInteractable();
            }

            jar.MoveTo(_jarSpillingPosition.position).OnComplete(() =>
            {
                jar.SpillAnimation();
                OnJarSpill?.Invoke(jar);
            });
        }

        // Move the milk bottle aside
        public void MoveBottleAside(MilkBottle bottle)
        {
            bottle.MoveTo(CalculateBottleDestination())
                .OnComplete(() =>
                {
                    _jarTrigger.OnTriggerEnter += OnSpillTrigger;
                    BottleMoved.Invoke();
                });

            _soundSystem.PlaySound(SUCCESS);
            _fxSystem.PlayEffect(SUCCESS, _spillFxPosition.position);
            _bottleStep++;
        }

        // Calculate the destination position for the milk bottle
        private Vector3 CalculateBottleDestination()
        {
            return new Vector3(_filledBottlePosition.position.x - _bottleStep, _filledBottlePosition.position.y, _filledBottlePosition.position.z);
        }

        // Invoke events when arrived at the shop
        private void InvokeOnArrived()
        {
            ConstructJars();
            ShopArrived?.Invoke();
        }

    }
}

