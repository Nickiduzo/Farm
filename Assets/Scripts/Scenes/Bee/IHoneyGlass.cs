using DG.Tweening;
using UnityEngine;

namespace Bee
{
    public interface IHoneyGlass
    {
        Tween Store(Transform point, float duration = 1f);
        void MakeVisualPartOf(SpriteRenderer renderer);
        Tween Open();
        Tween Close();
        Tween GoToDestination();
        void SetDestination(Vector3 destination);
        Tween FillWithHoney();
        void MakeInteractable();
        void MakeNonInteractable();
        void InitOnDragEnd();
        void ShowSuccess();
    }
}