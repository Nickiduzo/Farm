using System;
using AwesomeTools.Inputs;
using UnityEngine;
using AwesomeTools;

namespace SunflowerScene
{
    public class WaterPump : MonoBehaviour
    {
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private WaterPumpStreamPrefab _waterPump;
        [SerializeField] private Vector2 _clampedYPosition;
        [SerializeField] private SunflowerSprinkler _sunflowerSprinkler;

        public bool IsDraggable
        {
            set { 
                _dragAndDrop.IsDraggable = value;
                _waterPump.DisableWater();
            }
        }
        
        public event Action StartedFlow;
        public event Action StoppedFlow;

        public Vector3 WaterPumpHintPoint { get; private set; }

        // Sets the destination point for the WaterPumpHint.
        public void Construct(Vector3 destination, InputSystem input)
        {
            WaterPumpHintPoint = destination;
            _dragAndDrop.Construct(input, _clampedYPosition);
            _destinationOnDragEnd.Construct(destination);
            _dragAndDrop.OnDragStart += _waterPump.EnableWater;
            _dragAndDrop.OnDragEnded += _waterPump.DisableWater;
            _dragAndDrop.OnDragStart += StartFlow;
            _dragAndDrop.OnDragEnded += StopFlow;

            _destinationOnDragEnd.MoveToDestination();

            _sunflowerSprinkler.Initialize(_dragAndDrop, StartFlow, StopFlow);
        }

        // Unsubscribes from drag-related events when the object is destroyed.
        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= _waterPump.EnableWater;
            _dragAndDrop.OnDragEnded -= _waterPump.DisableWater;
        }

        // Invokes the StartedFlow event.
        private void StartFlow()
        {
            StartedFlow?.Invoke();
        }

        // Invokes the StoppedFlow event.
        private void StopFlow()
        {
            StoppedFlow?.Invoke();
        }

    }
}