using DG.Tweening;
using AwesomeTools.Inputs;
using System;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

namespace Carrot
{
    public class WaterPump : MonoBehaviour
    {
        [SerializeField] private WaterPumpStreamPrefab _waterStream;
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private SpriteRenderer _pipeSprite;
        [SerializeField] private Vector2 _clampedYPositionOnDrag;
        [SerializeField] private BaseSprinkler _sprinkler;

        public Vector3 Destination { get; private set; }
        private Vector3 _start;

        public event Action OnPosition;


        // set up a few parameters, subscribe all Actions [DragAndDrop], initialize [WaterPump] in [Sprinkler]
        public void Construct(Vector3 destination, Vector3 start, InputSystem input)
        {
            _start = start;
            Destination = destination;
            DisablePipeAnim();
            _dragAndDrop.Construct(input, _clampedYPositionOnDrag);

            _dragAndDrop.OnDragStart += _waterStream.EnableWater;
            _dragAndDrop.OnDragEnded += _waterStream.DisableWater;
            _dragAndDrop.OnDragStart += EnablePipeAnim;
            _dragAndDrop.OnDragEnded += DisablePipeAnim;
            _destinationOnDragEnd.Construct(destination);

            _sprinkler.Init(_dragAndDrop);
        }

        // unsubscribe all Actions
        private void OnDestroy()
        {
            _dragAndDrop.OnDragStart -= _waterStream.EnableWater;
            _dragAndDrop.OnDragEnded -= _waterStream.DisableWater;
            _dragAndDrop.OnDragStart -= EnablePipeAnim;
            _dragAndDrop.OnDragEnded -= DisablePipeAnim;
        }

        // set "false" fro [DragAndDrop], turn off water stream, disable all Tweens, leave scene
        public void EndLifeCycle()
        {
            DragAndDropState(false);
            MoveTo(_start);
            _waterStream.DisableWater();
            DOTween.Kill(gameObject);
        }
        
        // turn off/on pipe animation 
        private void EnablePipeAnim()
            => _pipeSprite.enabled = true;
        private void DisablePipeAnim()
            => _pipeSprite.enabled = false;

        // [WaterPump] has arrived to scene
        public void MoveToDestination()
            => MoveTo(Destination);
        
        // move to some point based on [targetPoint], after move invoke Action [OnPosition] that object has arrived at the target
        private void MoveTo(Vector3 targetPoint)
            => transform.DOMove(targetPoint, 1f).OnComplete(() =>
            {
                OnPosition?.Invoke();
            });

        // disable hint when first pick up 
        private void OnMouseDown()
            => HintSystem.Instance.HidePointerHint();
        
        // set true/false for [DragAndDrop] based on [value] 
        private void DragAndDropState(bool value)
            => _dragAndDrop.IsDraggable = value;
    }
}