using UnityEngine;

namespace SunflowerScene
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class VerticalPositionLayerSwitcher : MonoBehaviour
    {
        [SerializeField] private int _layerMeans;
        [SerializeField] private SpriteRenderer[] spriteRenderers;

        public int _index;

        private void Awake()
        {
            if (spriteRenderers.Length == 0)
            {
                spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            }
        }

        // set sortingOrder based on which side is selected
        public void SwitchSortingOrder(VerticalPosition verticalPosition)
        {
            switch (verticalPosition)
            {
                case VerticalPosition.Top:
                    foreach (var spriteR in spriteRenderers)
                    {
                        spriteR.sortingOrder -= _layerMeans;
                    }

                    break;
                case VerticalPosition.Bottom:
                    foreach (var spriteR in spriteRenderers)
                    {
                        spriteR.sortingOrder += _layerMeans;
                    }

                    break;
            }
        }

        // increase SortingOrder for avoid sprite overlaying in pests
        public void ChangeSortingOrder()
        {
            foreach (var spriteR in spriteRenderers)
            {
                spriteR.sortingOrder += _index;
            }
        }

        public void ChangeSortingLayer(int _sortingLayerID)
        {
            foreach (var spriteR in spriteRenderers)
            {
                spriteR.sortingLayerID = _sortingLayerID;
            }
        }
    }
}