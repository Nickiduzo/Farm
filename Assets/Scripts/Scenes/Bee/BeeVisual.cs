using UnityEngine;

namespace Bee
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BeeVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        public int _index {get ; set ;}

        public void ChangeSortingOrder(int index)
        {
            _index = index;
            foreach (var spriteR in spriteRenderers)
            {
                spriteR.sortingOrder += _index;
            }
        }
    }
}
