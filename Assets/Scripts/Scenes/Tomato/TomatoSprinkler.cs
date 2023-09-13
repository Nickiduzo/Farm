using System;
using Tomato;
using UnityEngine;
using AwesomeTools;

public class TomatoSprinkler : BaseSprinkler
{
    [SerializeField] private TomatoSeedsTriggerObserver _observerTomato;
    [SerializeField] private Collider2D _colliderTomato;

    private Action<TomatoSeed> OnStartDrag;
    private Action<TomatoSeed> OnEndDrag;
    private Action<TomatoSeed> OnTriggerStay;

    // The Initialize method initializes the object with a DragAndDrop instance and callbacks for starting drag, ending drag, and trigger stay events
    public void Initialize(DragAndDrop dragAndDrop, Action<TomatoSeed> StartDrag, Action<TomatoSeed> EndDrag, Action<TomatoSeed> TriggerStay)
    {
        Init(dragAndDrop);

        OnStartDrag = StartDrag;
        OnEndDrag = EndDrag;
        OnTriggerStay = TriggerStay;
    }

    // The Init method is called internally to initialize the object with a DragAndDrop instance
    public override void Init(DragAndDrop dragAndDrop)
    {
        DragAndDrop = dragAndDrop;

        PostInit();
    }

    // The PostInit method is called after initialization and sets up additional actions
    protected override void PostInit()
    {
        SetActionsToDragAndDrop(onDragStart: RestoreCollider);
    }

    // The InitStream method sets up event handling for the interactions within the stream
    public override void InitStream()
    {
        _observerTomato.OnTriggerEnter += OnStartDrag;
        _observerTomato.OnTriggerExit += OnEndDrag;
        _observerTomato.OnTriggerStay += OnTriggerStay;
    }

    // The OnDestroy method unsubscribes from events when the object is destroyed
    private void OnDestroy()
    {
        _observerTomato.OnTriggerEnter -= OnStartDrag;
        _observerTomato.OnTriggerExit -= OnEndDrag;
        _observerTomato.OnTriggerStay -= OnTriggerStay;
    }

    // The RestoreCollider method restores the collider of the tomato object to an enabled state
    private void RestoreCollider()
    {
        _colliderTomato.enabled = true;
    }

}
