using UnityEngine;

namespace SunflowerScene
{
    public class Pointer : MonoBehaviour
    {
        [SerializeField] private ArrowHint _arrowHint;
        private IPointable _pointable;

        //Init IPointable
        public void Construct(IPointable pointable)
        {
            _pointable = pointable;
        }

        // Sets the position of the arrow hint
        public void PointOnPosition()
        {
            _arrowHint.transform.position = _pointable.GetPointPosition();
            _arrowHint.gameObject.SetActive(true);
        }

        // Hides the arrow hint
        public void Hide()
        {
            _arrowHint.Hide();
        }
    }
}
