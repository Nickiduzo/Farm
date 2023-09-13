using DG.Tweening;

namespace Bee
{
    public interface IHoneyComb
    {
        void OnRecycled();
        void MakeInteractable();
        void MakeNonInteractable();
        Tween MoveDownPerStep();
    }
}