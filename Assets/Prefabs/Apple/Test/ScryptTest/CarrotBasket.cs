using DG.Tweening;
using System;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Apple
{
    public class CarrotBasket : MonoBehaviour, IWinInvoker
    {
        public event Action OnWin;
        [SerializeField] private appleTest _config;
        [SerializeField] private SpriteRenderer _frontBasketSprite;
        [SerializeField] private CarrotTriggerObserver _observer;
        [SerializeField] private Transform _carrotStorePosition;
        [SerializeField] private float _storeOffSetX;

        private int _carrotStoreCount;
        private Vector3 _destination;
        private Vector3 _start;

        public void Construct(Vector3 destination, Vector3 start)
        {
            _destination = destination;
            _start = start;
        }
        private void Awake()
            => _observer.OnTriggerEnter += ProcessCarrot;

        private void OnDestroy()
            => _observer.OnTriggerEnter -= ProcessCarrot;

        private void ProcessCarrot(Carrot carrot)
        {
            _carrotStoreCount++;
            carrot.MakeNonInteractable();
            MakeChildOfBasket(carrot);
            carrot.transform.DOMove(GetStorePosition(), .5f)
                .OnComplete(GoToStart);
        }

        private void GoToStart()
        {
            if (!IsAllCarrotsStored()) return;

            MoveTo(_start)
                .OnComplete(() => OnWin?.Invoke());
        }


        public Tween MoveToDestination()
            => MoveTo(_destination);

        private Tween MoveTo(Vector3 targetPoint)
            => transform.DOMove(targetPoint, 1f);

        private bool IsAllCarrotsStored()
            => _carrotStoreCount >= _config.MaxMoleToSpawn + 1;

        private void MakeChildOfBasket(Carrot carrot)
        {
            carrot.transform.SetParent(transform);
            MakeVisualChildOfBasket(carrot);
        }

        // Make carrot pseudo basket visual child;

        private void MakeVisualChildOfBasket(Carrot carrot)
            => carrot.GetComponent<SpriteRenderer>().sortingOrder = _frontBasketSprite.sortingOrder - 1;

        private Vector3 GetStorePosition()
            => _carrotStorePosition.position + GetRandomOffSet();

        private Vector3 GetRandomOffSet()
            => new(Random.Range(-_storeOffSetX, _storeOffSetX), 0, 0);
    }
}