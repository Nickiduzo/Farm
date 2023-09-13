using DG.Tweening;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEditor;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Apple
{
    public class AppleWaterPump : MonoBehaviour, IHintable
    {
        [SerializeField] private WaterPumpStreamPrefab _waterPumpStreamPrefab;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private AwesomeTools.DragAndDrop _dragAndDrop;
        [SerializeField] private SpriteRenderer _sprite;
        [SerializeField] private SpriteRenderer _spritePipe;

        [SerializeField] private BaseSprinkler _appleSprinkler;

        [SerializeField] private Vector2 _clampedYPositionOnDrag;

        private Vector3 _destination;
        private Vector3 _start;

        private AppleWaterPumpStream _appleWaterPumpStream;
        private Vector3 HolePos { get; set; }

        //Constructs waterpump
        public void Construct(Vector3 destination, Vector3 start, InputSystem input, Vector3 holePos)
        {
            _start = start;
            _destination = destination;
            _dragAndDrop.Construct(input, _clampedYPositionOnDrag);
            HolePos = holePos;

            _dragAndDrop.OnDragStart += _waterPumpStreamPrefab.EnableWater;
            _dragAndDrop.OnDragEnded += _waterPumpStreamPrefab.DisableWater;
            _appleWaterPumpStream = new AppleWaterPumpStream(_sprite, _spritePipe, _appleSprinkler);
            _destinationOnDragEnd.Construct(destination);

            ActivateHint();

            _appleSprinkler.Init(_dragAndDrop);
        }

        // It unsubscribes from events
        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= _waterPumpStreamPrefab.EnableWater;
            _dragAndDrop.OnDragEnded -= _waterPumpStreamPrefab.DisableWater;
        }

        // Deactivate the hint pointer
        public void DeactivateHint()
        {
            HintSystem.Instance.HidePointerHint();
        }

        // End the life cycle of the object
        public void EndLifeCycle()
        {
            _dragAndDrop.IsDraggable = false;
            _appleWaterPumpStream.Dispose();
            DOTween.Kill(gameObject);
            MoveTo(_start);
        }

        // Move to the destination point
        public void MoveToDestination()
            => MoveTo(_destination);

        // Move the object to the specified target point
        private void MoveTo(Vector3 targetPoint)
            => transform.DOMove(targetPoint, 1f);

        // Activate the hint pointer
        public void ActivateHint()
            => HintSystem.Instance.ShowPointerHint(_destination, HolePos);
    }
}