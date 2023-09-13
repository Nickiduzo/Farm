using UnityEngine;
using DG.Tweening;

namespace Quest
{
    public class QuestCar : MonoBehaviour
    {
        [SerializeField] private Animator _carAnimator;
        [SerializeField] private Transform _backWheel;
        [SerializeField] private Transform _frontWheel; 
        private float _currentZRotation;

        // uses to change oscillation depending on whether car is moving or standing
        public void ReducedOscillation()
            => _carAnimator.CrossFade("ReducedOscillation", 0.3f);
        public void NormalOscillation()
            => _carAnimator.CrossFade("NormalOscillation", 0.3f);

        // uses when the car moves back a little and then moves forward until it leaves scene
        public void WheelRolling()
        {
            Debug.Log("WheelRolling");
            _currentZRotation = _frontWheel.localEulerAngles.z;
            Sequence sequence = DOTween.Sequence();

            sequence.Append(_frontWheel.DORotate(new Vector3(0, 0, _currentZRotation + 180), 2f, RotateMode.FastBeyond360));
            sequence.Join(_backWheel.DORotate(new Vector3(0, 0, _currentZRotation + 180), 2f, RotateMode.FastBeyond360));
            sequence.AppendCallback(() => WheelRollingForward());
        }

        // uses when the car moves from left to center of scene and then creates task when it stops, also for leaving the scene
        public void WheelRollingForward()
        {
            Debug.Log("WheelRollingForward");
            _frontWheel.DORotate(new Vector3(0, 0, _currentZRotation - 730), 5.5f, RotateMode.FastBeyond360);
            _backWheel.DORotate(new Vector3(0, 0, _currentZRotation - 730), 5.5f, RotateMode.FastBeyond360);
        }
    }
}

