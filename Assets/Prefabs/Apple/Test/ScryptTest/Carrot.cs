using DG.Tweening;
using AwesomeTools.Inputs;
using UnityEngine;
using AwesomeTools;

namespace Apple
{
    public class Carrot : MonoBehaviour
    {
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;

        public void Construct(InputSystem inputSystem)
        {
            _dragAndDrop.Construct(inputSystem);
            _destinationOnDragEnd.Construct(transform.position);
            MakeNonInteractable();
        }

        public void MakeInteractable()
        {
            GetComponent<Collider2D>().enabled = true;
            _dragAndDrop.IsDraggable = true;
            _dragAndDrop.enabled = true;
            _destinationOnDragEnd.enabled = true;
        }
        public void MakeNonInteractable()
        {
            DOTween.Kill(gameObject);
            GetComponent<Collider2D>().enabled = false;
            _dragAndDrop.IsDraggable = false;
            _dragAndDrop.enabled = false;
            _destinationOnDragEnd.enabled = false;
        }
    }
}