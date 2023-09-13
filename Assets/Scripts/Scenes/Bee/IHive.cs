using System;
using UnityEngine;

namespace Bee
{
    public interface IHive
    {
        event Action OnOpened;
        event Action OnStored;
        IHoneyComb HoneyComb { get; }
        bool IsOpened { get; }
        Vector3 StayPosition { get; }
        void Init();
    }
}