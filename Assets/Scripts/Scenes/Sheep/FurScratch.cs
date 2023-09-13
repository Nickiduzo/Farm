using DG.Tweening;
using System;
using UnityEngine;
using UsefulComponents;

namespace Sheep
{
    public class FurScratch : MonoBehaviour
    {
        private static bool _isFirstFurWasTrimmed = false;
        private static int _lastFurOrderInLayer = 9;
        public event Action OnTrimmed;
        public bool IsTrimmed => _isTrimmed;

        [SerializeField] private float _shakeDuration;
        [SerializeField] private float _shakingStrength;
        [SerializeField] private int _shakingVibrato;
        [SerializeField] private float _desiredProgress;

        private float _progress;

        private bool _isTrimmed;

        private FurStorage _furStorage;
        private SheepAnimator _animator;
        public Tween ShakingTween;

        //Constructs FurScratch
        public void Construct(FurStorage furStorage, SheepAnimator animator)
        {
            _furStorage = furStorage;
            _animator = animator;
        }

        // Shake the object by starting a shaking tween animation
        public void Shake()
            => ShakingTween = DoShakingTween();

        // Perform the shaking tween animation
        private Tweener DoShakingTween()
            => transform.DOShakePosition(_shakeDuration, _shakingStrength, _shakingVibrato, 45, false,
                true,
                ShakeRandomnessMode.Harmonic).OnPlay(IncreaseProgress).OnComplete(CheckProgress);

        // Get the SheepAnimator component attached to this object
        public SheepAnimator GetSheepAnimator()
        {
            return _animator;
        }

        // Increase the progress value and hide the pointer hint
        private void IncreaseProgress()
        {
            HintSystem.Instance.HidePointerHint();
            _progress += Time.fixedDeltaTime;
        }

        // Check the progress and take action accordingly
        private void CheckProgress()
        {
            if (_isTrimmed)
            {
                return;
            }

            if (_progress >= _desiredProgress)
            {
                if(_isFirstFurWasTrimmed){
                    for(int i = 0; i < transform.GetChild(0).childCount; i++){
                        transform.GetChild(0).GetChild(i).GetComponent<SpriteRenderer>().sortingOrder = ++_lastFurOrderInLayer;
                    }
                }else{
                    _isFirstFurWasTrimmed = true;
                }
                _isTrimmed = true;
                OnTrimmed?.Invoke();
                ShakingTween.Kill();
                _furStorage.MoveToStorage(this);
                return;
            }

            ShakingTween = DoShakingTween();
        }

    }
}