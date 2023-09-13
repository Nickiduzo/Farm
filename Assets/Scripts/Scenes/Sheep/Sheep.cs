using DG.Tweening;
using AwesomeTools.Sound;
using System;
using System.Collections;
using UnityEngine;

namespace Sheep
{
    public class Sheep : MonoBehaviour
    {
        private const string Success = "Success";
        private const string SHEEP = "SheepVoice";

        public event Action OnArrived;
        public event Action OnNextSheep;
        public event Action OnWholeFurTrimmed;

        [SerializeField] private float _movingDuration;
        [SerializeField] private SheepAnimator _animator;
        [SerializeField] private GameObject _interactableFurObj;

        private Vector3 _startPoint;
        private FurStorage _furStorage;
        private int _furCount;
        private int _trimmedFurCount;
        private Vector3 _destination;
        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;
        private Coroutine _idleAnimationTurnOnRoutine;

        // Initializes the Sheep with the specified parameters and starts the movement to the start point
        public void Construct(Vector3 startPoint, Vector3 destination,
            FurStorage furStorage, ISoundSystem soundSystem, FxSystem fxSystem)
        {
            _destination = destination;
            _furStorage = furStorage;
            _startPoint = startPoint;
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
            ConstructFurScratches();
            MoveToStartPoint()
                .OnComplete(InvokeOnArrived);
        }

        // Construct the fur scratches by iterating through all the FurScratch components in the object's children
        private void ConstructFurScratches()
        {
            foreach (var fur in transform.GetComponentsInChildren<FurScratch>())
            {
                _furCount++;
                fur.OnTrimmed += FurTrimmed;
                fur.Construct(_furStorage, _animator);
            }
        }

        // Event handler for when a fur scratch is trimmed
        private void FurTrimmed()
        {
            _trimmedFurCount++;
            IsNeedToShowTongue();

            if (!IsWholeFurTrimmed())
            {
                return;
            }

            _animator.PlayWalk();
            OnWholeFurTrimmed?.Invoke();
            _soundSystem.PlaySound(Success);
            _fxSystem.PlayEffect(Success, transform.position);
            MoveToDestination().OnComplete(NextSheep);
        }

        // Check if the tongue needs to be shown based on the trimmed fur count
        private void IsNeedToShowTongue()
        {
            if (_trimmedFurCount >= _furCount - 2 && _trimmedFurCount < _furCount)
            {
                SheepAnimator.IsTrimLastFur = true;
                SheepAnimator.IsTrimmerDragged = true;
                _animator.PlayTrimLastFur();
            }
            else
            {
                SheepAnimator.IsTrimLastFur = false;
            }
        }

        // Move to the next sheep and deactivate the current sheep
        private void NextSheep()
        {
            gameObject.SetActive(false);
            OnNextSheep?.Invoke();
        }

        // Tween the movement of the sheep to the destination point
        private Tween MoveToDestination()
            => transform.DOMove(_destination, _movingDuration);

        // Check if the whole fur is trimmed
        private bool IsWholeFurTrimmed()
            => _trimmedFurCount >= _furCount;

        // Tween the movement of the sheep to the start point
        private Tween MoveToStartPoint()
        {
            _idleAnimationTurnOnRoutine ??= StartCoroutine(TurnOnIdle());
            return transform.DOMove(_startPoint, _movingDuration);
        }

        private IEnumerator TurnOnIdle()
        {
            yield return new WaitForSeconds(_movingDuration - .3f);
            SheepAnimator.IsIdlePlayed = false;
            _animator.PlayIdle();
            StopCoroutine(_idleAnimationTurnOnRoutine);
            _idleAnimationTurnOnRoutine = null;
        }

        // Invoke the event when the sheep arrives at its destination
        private void InvokeOnArrived()
        {
            for (int i = 0; i < _interactableFurObj.transform.childCount; i++)
            {
                _interactableFurObj.transform.GetChild(i).GetComponent<Collider2D>().enabled = true;
            };
            OnArrived?.Invoke();
            _soundSystem.PlaySound(SHEEP);
        }

        // Deactivate the object when it becomes invisible
        private void OnBecameInvisible()
        {
            gameObject.SetActive(false);
        }

    }
}