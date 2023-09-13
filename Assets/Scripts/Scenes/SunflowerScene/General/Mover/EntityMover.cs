using DG.Tweening;
using UnityEngine;

namespace SunflowerScene
{
    public class EntityMover : MonoBehaviour
    {
        protected const float DefaultExitDuration = 1f;
        [SerializeField] protected float _speed;
        [SerializeField] private bool rotateWheels;
        [SerializeField] private Transform[] wheels;
        [SerializeField] private float[] round;

        public Tween MoveTo(Vector3 target)
            => transform.DOMove(target, GetMovingDuration(target));

        public void Flip()
        {
            float currentRotation = transform.rotation.eulerAngles.y;
            float targetRotation = (currentRotation == 0) ? 180 : 360 - currentRotation;
            transform.Rotate(0, targetRotation, 0);
        }

        public void ExitScene(Vector3 exitPosition) =>
            ExitScene(transform, exitPosition, GetMovingDuration(exitPosition));

        public static void ExitScene(Transform transform, Vector3 exitPosition, float duration = DefaultExitDuration)
        {
            transform.DOMove(exitPosition, duration);
        }

        private float GetMovingDuration(Vector3 targetPosition)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            float timeToTarget = distance / _speed;
            if (rotateWheels)
            {
                for (int i = 0; i < round.Length; i++)
                {
                    float rotateM = 100 / round[i]; // кол-во оборотов в 1 метре
                    float speedRot = distance * (rotateM * 120);
                    wheels[i].DOLocalRotate(new Vector3(0, 0, -speedRot), timeToTarget-.2f, RotateMode.LocalAxisAdd);
                }
            }

            return timeToTarget;
        }
    }
}