using DG.Tweening;
using AwesomeTools.Inputs;
using UnityEngine;
using AwesomeTools;

namespace Apple
{
    public class WaterPump : MonoBehaviour
    {
        [SerializeField] private WaterPumpStream _waterStream;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        private Vector3 _destination;
        private Vector3 _start;

        public void Construct(Vector3 destination, Vector3 start, InputSystem input)
        {
            _start = start;
            _destination = destination;
            _dragAndDrop.Construct(input);

            _destinationOnDragEnd.Construct(destination);
            
            
            Sprinkler.Instance.InitApple(_dragAndDrop);
        }

        public void EndLifeCycle()
        {
            DOTween.Kill(gameObject);
            MoveTo(_start);
        }

        public void MoveToDestination()
            => MoveTo(_destination);

        private void MoveTo(Vector3 targetPoint)
            => transform.DOMove(targetPoint, 1f).SetLink(gameObject);
    }
}