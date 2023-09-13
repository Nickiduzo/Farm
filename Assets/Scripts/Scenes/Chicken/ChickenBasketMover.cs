using System;
using AwesomeTools.Inputs;
using UnityEngine;
using AwesomeTools;

namespace ChickenScene
{
    public class ChickenBasketMover : MonoBehaviour
    {
        [SerializeField] private MouseTrigger _mouseTrigger;

        public bool IsDraggable;
        public event Action OnDragStart;
         
        private InputSystem _inputSystem;
        private Vector2 _clampedXPosition;


        // set barriers and input system
        public void Construct(InputSystem inputSystem, Vector2 clampedXPosition = default(Vector2))
        {
            _inputSystem = inputSystem;
            _clampedXPosition = clampedXPosition;
        }
        
        // monitor whether basket has been dragged
        private void Awake()
        {
            _mouseTrigger.OnDrag += CalculateDrag;
            _mouseTrigger.OnDrag += DragStart;
        }
        
        private void OnDestroy()
        {
            _mouseTrigger.OnDrag -= CalculateDrag;
            _mouseTrigger.OnDrag -= DragStart;
        }
        
        // set boundaries and prohibit basket from moving beyond barriers
        private void CalculateDrag()
        {
            if (!IsDraggable) return;
            Vector3 newPosition = _inputSystem.CalculateTouchPosition();
            if (_clampedXPosition != default(Vector2))
            {
                newPosition.x = Mathf.Clamp(newPosition.x, _clampedXPosition.x, _clampedXPosition.y);
            }
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
        
        // basket has been dragged
        private void DragStart()
        {
            if (!IsDraggable) return;
        
            OnDragStart?.Invoke();
        }
    }
}