using CowScene;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HayController : MonoBehaviour
{
    public Action<bool> OnAnyHayDrag;
    private List<Hay> _hays = new List<Hay>();

    // Initializes a hay object and adds it to the list of hays
    public void InitHay(Hay hay)
    {
        _hays.Add(hay);
        hay.OnDrag += SetInteractive;
    }

    // Sets the interactiveness of hays based on the drag state of a hay object
    public void SetInteractive(bool onDrag, Hay hayAction)
    {
        OnAnyHayDrag?.Invoke(onDrag);

        foreach (var hay in _hays)
        {
            if (hay != hayAction)
            {
                hay.SetInteractable(!onDrag);
            }
        }
    }
    // It unsubscribes from events
    private void OnDisable()
    {
        foreach (var hay in _hays)
        {
            hay.OnDrag -= SetInteractive;
        }
    }
}
