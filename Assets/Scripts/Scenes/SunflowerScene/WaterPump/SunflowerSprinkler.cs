using System;
using SunflowerScene;
using UnityEngine;
using AwesomeTools;

public class SunflowerSprinkler : BaseSprinkler
{
    private Action OnStartedFlow;
    private Action OnStoppedFlow;

    [SerializeField] private FlowView _flowView;
    [SerializeField] private DragAndDrop _dragAndDrop;
    [SerializeField] private WaterPumpTriggerObserver _observerSunflower;


    // The Initialize method initializes the object with a DragAndDrop instance and callbacks for starting and stopping the flow
    public void Initialize(DragAndDrop dragAndDrop, Action StartedFlow, Action StoppedFlow)
    {
        Init(dragAndDrop);

        OnStartedFlow = StartedFlow;
        OnStoppedFlow = StoppedFlow;
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
        _observerSunflower.OnTriggerStay += PrecessWatering;

        SetActionsToDragAndDrop(ShowFlow, onDragEnd: StopFlow);
    }

    public override void InitStream()
    {
        
    }

    // The PrecessWatering method processes watering for a growing plant if it is currently in the growing state
    private void PrecessWatering(PlantGrowing plant)
    {
        if (plant.Growing)
        {
            plant.ProcessWatering();
        }
    }


    // The ShowFlow method shows the flow and invokes the callback for starting the flow
    private void ShowFlow()
    {
        _flowView.ShowFlow();
        OnStartedFlow?.Invoke();
    }

    // The StopFlow method hides the flow and invokes the callback for stopping the flow
    private void StopFlow()
    {
        _flowView.HideFlow();
        OnStoppedFlow?.Invoke();
    }
}
