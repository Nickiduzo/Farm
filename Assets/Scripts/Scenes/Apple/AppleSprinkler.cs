using Apple;
using AwesomeTools;
using UnityEngine;

public class AppleSprinkler : BaseSprinkler
{
    [SerializeField] private AppleSeedlingTriggerObserver _appleSeedlingTriggerObserverApple;
    [SerializeField] private BaseHoleTriggerObserver _observerApple;
    [SerializeField] private Collider2D _colliderAppleStream;
    [SerializeField] private SpriteRenderer _spriteRendererAppleStream;
    [SerializeField] private SpriteRenderer _spriteRendererAppleStreamPipe;

    // The Init method takes a DragAndDrop object and initializes it
    public override void Init(DragAndDrop dragAndDrop)
    {
        DragAndDrop = dragAndDrop;

        PostInit();
    }


    // The InitStream method sets up event handling for interacting with objects inside the stream
    public override void InitStream()
    {
        if (_observerApple != null)
        {
            _observerApple.OnTriggerStay += PureHole;
        }

        _appleSeedlingTriggerObserverApple.OnTriggerEnter += StartWateringHole;
        _appleSeedlingTriggerObserverApple.OnTriggerExit += StopWateringHole;
    }

    // It unsubscribes from events
    private void OnDestroy()
    {
        if (_observerApple != null)
        {
            _observerApple.OnTriggerStay -= PureHole;
        }

        if (_appleSeedlingTriggerObserverApple != null)
        {
            _appleSeedlingTriggerObserverApple.OnTriggerEnter -= StartWateringHole;
            _appleSeedlingTriggerObserverApple.OnTriggerEnter -= StopWateringHole;
        }
    }

    // The PostInit method is called after initialization and sets up additional actions
    protected override void PostInit()
    {
        SetActionsToDragAndDrop(onDragStart: DeactivateHint);
        SetActionsToDragAndDrop(onDragStart: StartWateringVisual, onDragEnd: StopWateringVisual);
    }

    // The EnableWaterApple method enables watering the apple
    private void EnableWaterApple()
    {
        EnableWater(_colliderAppleStream, _spriteRendererAppleStream);
        _spriteRendererAppleStreamPipe.enabled = true;
    }

    // The DisableWaterApple method disables watering the apple
    private void DisableWaterApple()
    {
        DisableWater(_colliderAppleStream, _spriteRendererAppleStream);
        _spriteRendererAppleStreamPipe.enabled = false;
    }

    // The StartWateringVisual method starts the visual effects of watering
    private void StartWateringVisual()
    {
        StartSound();
        EnableWaterApple();
    }

    // The StopWateringVisual method stops the visual effects of watering
    private void StopWateringVisual()
    {
        StopSound();
        DisableWaterApple();
    }

    // The StartWateringHole method starts the growth process of apple seedlings when watering
    private void StartWateringHole(SeedlingsApple seedlingsApple)
    {
        seedlingsApple.StopWaterHint();
        seedlingsApple.StartGrow();
    }

    // The StopWateringHole method stops the growth process of apple seedlings when watering is stopped
    private void StopWateringHole(SeedlingsApple seedlingsApple)
    {
        seedlingsApple.StopGrow();
    }

}
