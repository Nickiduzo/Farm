using DG.Tweening;
using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace CowScene
{
    public class MilkBasket : MonoBehaviour
    {
        private const string SUCCESS = "Success";

        public Action AllJarStored;
        public Action BasketArrived;

        [SerializeField] private Transform[] _jarPosition;
        [SerializeField] private Transform _startStoreTransform;
        [SerializeField] private MilkBasketTriggerObserver _basketTrigger;
        [SerializeField] private SpriteRenderer _frontBasketSprite;

        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;

        private Vector3 _destination;
        private Vector3 _end;

        private int _currentJarCount;
        private int _jarConfigCount;
        private int _jarPositionNum;

        //its init MilkBasket
        public void Construct(Vector3 destination, Vector3 end, ISoundSystem soundSystem, FxSystem fxSystem, int jarConfigCount)
        {
            _jarConfigCount = jarConfigCount;
            _destination = destination;
            _end = end;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;

            MoveTo(_destination)
                .OnComplete(InvokeOnArrived);
        }

        // Invoke events when arrived at the basket
        private void InvokeOnArrived()
        {
            BasketArrived?.Invoke();
            _basketTrigger.OnTriggerEnter += StoreJar;
        }

        // Triggered when all jars are stored in the basket
        private void OnAllJarsStored()
        {
            _basketTrigger.OnTriggerEnter -= StoreJar;
            AllJarStored?.Invoke();
        }

        // Store a jar in the basket
        private void StoreJar(Jar jar)
        {
            _soundSystem.PlaySound(SUCCESS);
            _fxSystem.PlayEffect(SUCCESS, transform.position);

            Sequence sequence = DOTween.Sequence();
            sequence.AppendCallback(jar.MakeNonInteractable);
            sequence.Append(jar.MoveTo(_startStoreTransform.position)
                .OnComplete(() => jar.MoveInSortingOrder(_frontBasketSprite.sortingOrder - _jarConfigCount + _jarPositionNum)));
            sequence.Append(jar.MoveTo(_jarPosition[_jarPositionNum].position));
            sequence.OnComplete(() => 
            {
                _currentJarCount++;
                MakeChildOfBasket(jar);

                if (_currentJarCount >= _jarConfigCount)
                {
                    MoveTo(_end)
                        .OnComplete(OnAllJarsStored);
                }
            });

            _jarPositionNum++;

            sequence.Play();
        }

        // Make a jar a child of the basket
        private void MakeChildOfBasket(Jar jar)
        {
            jar.transform.SetParent(transform);
        }

        // Move to a specific target point with optional speed
        private Tween MoveTo(Vector3 targetPoint, float speed = 1f)
        {
            return transform.DOMove(targetPoint, speed);
        }
    }
}

