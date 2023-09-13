using DG.Tweening;
using UnityEngine;

namespace SunflowerScene
{
    public class CarMove : EntityMover
    {
        [SerializeField] private Ease _ease = Ease.InQuart;

        
        public Tween MoveTo(Vector3 target)
            => transform.DOMove(target, GetMovingD(target)).SetEase(_ease);

        public void Flip()
        {
            float currentRotation = transform.rotation.eulerAngles.y;
            float targetRotation = (currentRotation == 0) ? 180 : 360 - currentRotation;
            transform.Rotate(0, targetRotation, 0);
        }

        public void ExitScene(Vector3 exitPosition) => ExitScene(transform, exitPosition, GetMovingD(exitPosition));

        public static void ExitScene(Transform transform, Vector3 exitPosition, float duration = DefaultExitDuration)
        {
            transform.DOMove(exitPosition, duration)
                .OnComplete(() => transform.gameObject.SetActive(false));
        }

        private float GetMovingD(Vector3 targetPosition)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            float timeToTarget = distance / _speed;
           
            return timeToTarget;
        }
    }
}