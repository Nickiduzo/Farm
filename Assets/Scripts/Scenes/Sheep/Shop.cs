using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UsefulComponents;

namespace Sheep
{
    public class Shop : MonoBehaviour
    {
        private const int TRIANGLE_SIZE = 4;
        private const float TRIANGLE_Y_OFFSET = 1.5f;
        private const float TRIANGLE_X_OFFSET = 1.9f;
        public static Coroutine PointerHintRoutine;
        public event Action<Vector3, Vector3> OnArrived;

        [SerializeField] float Frequency;
        [SerializeField] float Amplitude;
        [SerializeField] float YOffSet;
        [SerializeField] float XOffset;

        [SerializeField] private float _movingDuration;
        [SerializeField] private float _storeOffSet;
        [SerializeField] private StoreContainer _storeContainer;
        [SerializeField] private RotateHint _rotateHint;
        [SerializeField] private Transform _composedFurSpawnPoint;
        [SerializeField] private Transform _composedFurDestination;
        [SerializeField] private Transform _rotateHintTrans;

        private FurStorage _furStorage;
        private Vector3 _startPoint;
        private int _shopOrderInLayer = 5;
        private int _rotateHintOrderInLayer = 52;

        //Constructs SHOP
        public void Construct(FurStorage furStorage, Vector3 startPoint)
        {
            _rotateHint.Disappear();
            _startPoint = startPoint;
            _furStorage = furStorage;
            MoveToStartPoint()
                .OnComplete(MoveFurToStorePoint);
        }

        // Move fur to the store point
        private void MoveFurToStorePoint()
        {
            _rotateHintTrans.GetComponent<SpriteRenderer>().sortingOrder = _rotateHintOrderInLayer;
            transform.GetComponent<SpriteRenderer>().sortingOrder = _shopOrderInLayer;
            List<Vector3> list = new();

            // Calculate the positions for fur placement
            for (var i = 0; i < TRIANGLE_SIZE; i++)
            {
                for (var j = i; j < 2 * 4 - i - 1; j++)
                {
                    list.Add(new Vector3(_storeOffSet * j - TRIANGLE_X_OFFSET, _storeOffSet * i - TRIANGLE_Y_OFFSET, 0));
                }
            }

            list.Reverse();

            var sequence = DOTween.Sequence();
            var index = 0;

            // Move each fur to its corresponding position
            foreach (var fur in _furStorage.CaughtFur)
            {
                fur.transform.DOMove(list[index++], _movingDuration)
                    .OnComplete(() => fur.transform.SetParent(_storeContainer.transform.GetChild(0)));
            }

            sequence.Append(_rotateHint.Appear());
            sequence.AppendCallback(OnArrive);
        }

        // Start the routine for moving the pointer hint
        private void StartRoutine()
        {
            HintSystem.Instance.ShowPointerHint(Camera.main.ScreenToWorldPoint(_rotateHint.transform.position), null, 0.5f);
            PointerHintRoutine ??= StartCoroutine(MovePointerHint());
        }

        // Coroutine for moving the pointer hint
        private IEnumerator MovePointerHint()
        {
            var HintSystemObj = GameObject.Find("HintSystem");
            while (true)
            {
                yield return new WaitForFixedUpdate();
                float x = Mathf.Cos(Time.time * Frequency) * Amplitude;
                float y = Mathf.Sin(Time.time * Frequency) * Amplitude;
                HintSystemObj.transform.position = new Vector2(x + XOffset, y + YOffSet);
            }
        }

        // Action to perform upon arrival
        private void OnArrive()
        {
            _storeContainer.IsDraggable = true;
            StartRoutine();
            OnArrived?.Invoke(_composedFurSpawnPoint.position, _composedFurDestination.position);
        }

        // Move to the start point tween
        private Tween MoveToStartPoint()
            => transform.DOMove(_startPoint, _movingDuration);

    }
}