using UnityEngine;
using UsefulComponents;

namespace Bee
{
    public class Bee : MonoBehaviour
    {
        [SerializeField] private MoveStartDestination _moveStartDestination;
        [SerializeField] private LoopMoving _loopMoving;
        [SerializeField] private Collider2D _collider;
        [SerializeField] private BeeVisual _beeVisual;

        //Make non-interactible a bee
        public void Catch()
        {
            _loopMoving.Kill();
            _collider.enabled = false;
        }

        public void IncreaseSortingOrder(int index)
        {
            _beeVisual.ChangeSortingOrder(index);
        }
    }
}