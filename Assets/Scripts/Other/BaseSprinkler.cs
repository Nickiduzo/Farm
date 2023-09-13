using System;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;
using AwesomeTools;

public abstract class BaseSprinkler : MonoBehaviour
{
    private readonly string _waterSound = "Water";

    protected DragAndDrop DragAndDrop;

    // Init Sprinkler and set DragAndDrop
    public abstract void Init(DragAndDrop dragAndDrop);

    //Post init, may event subscription
    protected abstract void PostInit();

    //Init stream , The implementation is specific to derived classes
    public abstract void InitStream();

    //its enables water by enabling the collider and sprite renderer components. It also starts the water sound
    protected void EnableWater(Collider2D _collider, SpriteRenderer _spriteRenderer)
    {
        _collider.enabled = true;
        _spriteRenderer.enabled = true;
        StartSound();
    }

    //its disables water by disabling the collider and sprite renderer components. It also stops the water sound
    protected void DisableWater(Collider2D _collider, SpriteRenderer _spriteRenderer)
    {
        _collider.enabled = false;
        _spriteRenderer.enabled = false;
        StopSound();
    }
    //sets actions to the DragAndDrop events
    protected void SetActionsToDragAndDrop(Action onDragStart = null, Action onDragStay = null, Action onDragEnd = null)
    {
        DragAndDrop.OnDragStart += onDragStart;
        DragAndDrop.OnDrag += onDragStay;
        DragAndDrop.OnDragEnded += onDragEnd;
    }

    //adds progress to a BaseHole object based on the delta time multiplied by 5
    protected void PureHole(BaseHole hole) => hole.AddProgress(Time.deltaTime * 5);

    //plays the water sound using the SoundSystemUser instance and the specified water sound
    protected void StartSound() => SoundSystemUser.Instance.PlaySound(_waterSound);

    //stops the water sound using the SoundSystemUser instance and the specified water sound
    protected void StopSound() => SoundSystemUser.Instance.StopSound(_waterSound);

    //hides the pointer hint using the HintSystem instance
    protected void DeactivateHint() => HintSystem.Instance.HidePointerHint();
}
