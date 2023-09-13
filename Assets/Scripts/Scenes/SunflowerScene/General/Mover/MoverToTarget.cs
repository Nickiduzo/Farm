using DG.Tweening;
using System;
using UnityEngine;

namespace SunflowerScene
{
    public class MoverToTarget : MonoBehaviour
    {
        [SerializeField] private EntityMover _mover;
        [SerializeField] private Transform _rotatePoint;
        [SerializeField] private Transform _endPoint;

        private Vector3 _destinationAfterRotate;
        private ITarget _target;

        public event Action StartMove;
        public event Action EndMove;


        // Sets the target for the object.
        public void Construct(ITarget target)
        {
            _target = target;
        }

        // Sets the end point and rotate point for the object's movement.
        public void Construct(Transform endPoint, Transform rotatePoint)
        {
            _rotatePoint = rotatePoint;
            _endPoint = endPoint;
        }

        // Moves the object to the target position, either by rotating first or directly moving.
        public void MoveToTarget()
        {
            OnStartMove();
            if (_target.LastOnWay)
            {
                Rotate(_target.Position());
            }
            else
            {
                MoveToTarget(_target.Position());
            }
        }

        // Moves the object to the specified target position.
        private void MoveToTarget(Vector3 targetPosition)
        {
            var moveTo = _mover.MoveTo(targetPosition);
            moveTo.OnComplete(OnEndMove);
        }

        // Rotates the object to a specific destination after rotating around a rotation point.
        private void Rotate(Vector3 destinationAfterRotate)
        {
            _destinationAfterRotate = destinationAfterRotate;
            var moveTo = _mover.MoveTo(_rotatePoint.position);
            moveTo.OnComplete(RotateAndMoveToDestination);
        }

        // Rotates the object and then moves it to the destination after rotation.
        private void RotateAndMoveToDestination()
        {
            _mover.Flip();
            MoveToTarget(_destinationAfterRotate);
        }

        // Moves the object to the end point, typically used to exit the scene.
        public void MoveToEnd()
        {
            _mover.ExitScene(_endPoint.position);
        }

        // Invokes the start move event.
        private void OnStartMove()
        {
            StartMove?.Invoke();
        }

        // Invokes the end move event.
        private void OnEndMove()
        {
            EndMove?.Invoke();
        }
    }
}