using DG.Tweening;
using AwesomeTools.Inputs;
using UnityEngine;
using AwesomeTools;

namespace SunflowerScene
{
    public class Sunflower : MonoBehaviour
    {
        [SerializeField] private MoveToDestinationOnDragEnd _destinationOnDragEnd;
        [SerializeField] private DragAndDrop _dragAndDrop;
        [SerializeField] private PlantGrowing _plantGrowing;
        [SerializeField] private SunflowerHead _sunflowerHead;
        [SerializeField] private Rigidbody2D _rigidbody2D;
        [SerializeField] private HoleTriggerObserver _holeTriggerObserver;

        [SerializeField] private SpriteRenderer[] spriteRenderers;
        [SerializeField] private bool changeOrderLayer;
        [SerializeField] private int layerInt;
        public SunflowerHead SunflowerHead => _sunflowerHead;
        public bool Planted{ get; private set; }
        public PlantGrowing PlantGrowing => _plantGrowing;

        private Transform _destination;


        // It subscribes to events

        private void Awake()
            => _holeTriggerObserver.OnTriggerEnter += ProcessHole;
        // It unsubscribes from events
        private void OnDestroy()
            => _holeTriggerObserver.OnTriggerEnter -= ProcessHole;
        // It unsubscribes from events
        private void OnDisable()
            => _plantGrowing.CountUp -= OnPlantGrew;
        

        // Constructs the Sunflower object with the specified destination and input system.
        public void Construct(Transform destination, InputSystem inputSystem)
        {
            _dragAndDrop.Construct(inputSystem);
            _destination = destination;
            _dragAndDrop.IsDraggable = false;
            _plantGrowing.CountUp += OnPlantGrew;
        }

        // Sets the simulated state of the Rigidbody2D component.
        public void SetRigidbodySimulated(bool value)
        {
            _rigidbody2D.simulated = value;
        }

        // Updates the destination on drag end if the sunflower is not planted.
        public void UpdateDestinationOnDragEnd()
        {
            if (Planted) return;
            _destinationOnDragEnd.Construct(_destination.position);
        }

        // Processes the interaction with a SunflowerHole object.
        private void ProcessHole(SunflowerHole hole)
        {
            if (changeOrderLayer)
                ChangeOrderLayer();
            Planted = true;
            MakeNonInteractable();
            hole.ProcessHole(this);
            ContactArea.Instance.MoveSeedToHole(hole._storePosition.position, gameObject.transform, 0.5f)
                .OnComplete(() =>
                {
                    hole.DigHole();
                    MakeNonInteractable();
                });
        }


        private void ChangeOrderLayer()
        {
            foreach (var spriteRenderer in spriteRenderers)
            {
                spriteRenderer.sortingOrder += layerInt;
            }
        }

        // Disables the drag-and-drop interaction, making the sunflower non-interactable.

        public void MakeNonInteractable()
        {
            _dragAndDrop.IsDraggable = false;
        }

        // Enables the drag-and-drop interaction, making the sunflower interactable.
        public void MakeInteractable()
        {
            if (Planted == false)
            {
                _dragAndDrop.IsDraggable = true;
            }
        }

        // Event handler for the CountUp event of the PlantGrowing component.
        private void OnPlantGrew(Transform plantGrowing)
        {
            SetRigidbodySimulated(false);
            _sunflowerHead.Appear();
        }

        // Activates or deactivates the flashing effect of the sunflower.
        public void ActivateFlash(bool isActive)
        {
            _plantGrowing.Flashes.SetFlashes(isActive);
        }
    }
}