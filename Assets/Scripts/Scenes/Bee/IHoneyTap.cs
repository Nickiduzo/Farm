using DG.Tweening;
using System;

namespace Bee
{
    public interface IHoneyTap
    {
        event Action OnHoneyStreamEnabled;
        void MakeInteractable();
        void MakeNonInteractable();
        void SetNewSortingOrder();
        Tween DisableHoneyStream();
    }
}