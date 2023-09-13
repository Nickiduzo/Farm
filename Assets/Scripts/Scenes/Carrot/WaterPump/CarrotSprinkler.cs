using UnityEngine;
using AwesomeTools;

public class CarrotSprinkler : BaseSprinkler
{
    [SerializeField] private BaseHoleTriggerObserver _baseHoleTriggerObserverCarrot;
    [SerializeField] private Collider2D _colliderCarrotStream;
    [SerializeField] private SpriteRenderer _spriteRendererCarrotStream;

    // The Init method takes a DragAndDrop object and initializes it
    public override void Init(DragAndDrop dragAndDrop)
    {
        DragAndDrop = dragAndDrop;

        PostInit();
    }

    // The InitStream method sets up event handling for staying in the trigger area of the carrot hole
    public override void InitStream()
    {
        _baseHoleTriggerObserverCarrot.OnTriggerStay += PureHole;
    }

    // The OnDestroy method unsubscribes from events when the object is destroyed
    private void OnDestroy()
    {
        _baseHoleTriggerObserverCarrot.OnTriggerStay -= PureHole;

    }

    // The PostInit method is called after initialization and sets up additional actions
    protected override void PostInit()
    {
        SetActionsToDragAndDrop(onDragStart: EnableWaterCarrot, onDragEnd: DisableWaterCarrot);
    }

    // The EnableWaterCarrot method enables watering for the carrot
    private void EnableWaterCarrot()
    {
        EnableWater(_colliderCarrotStream, _spriteRendererCarrotStream);
    }

    // The DisableWaterCarrot method disables watering for the carrot
    private void DisableWaterCarrot()
    {
        DisableWater(_colliderCarrotStream, _spriteRendererCarrotStream);
    }
}
