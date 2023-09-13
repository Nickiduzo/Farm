using UnityEngine;

namespace Sheep
{
    public class SheepAnimator : MonoBehaviour
    {
        public static bool IsTrimmerDragged = false;
        public static bool IsTrimLastFur = false;
        public static bool IsIdlePlayed = false;
        public static bool IsFurTrimming = false;

        private readonly int Walk = Animator.StringToHash("Walk");
        private readonly int Idle = Animator.StringToHash("Idle");
        private readonly int TrimLastFur = Animator.StringToHash("TrimLastFur");

        [SerializeField] private Animator _animator;
        [SerializeField] private Animator _eyeAnimator;

        // Play the walking animation
        public void PlayWalk()
        {         
            _animator.SetTrigger(Walk);
            IsIdlePlayed = false;
        }

        // Play the idle animation
        public void PlayIdle()
        {
            if (!IsIdlePlayed)
            {
                _animator.SetTrigger(Idle);
                _eyeAnimator.enabled = true;                
                IsIdlePlayed = true;
            }
        }

        // Play the trim last fur animation
        public void PlayTrimLastFur()
        {
            if (IsTrimmerDragged && IsTrimLastFur && IsFurTrimming)
            {
                _animator.SetTrigger(TrimLastFur);
                _eyeAnimator.enabled = false;
                IsIdlePlayed = false;
            }
        }

    }
}