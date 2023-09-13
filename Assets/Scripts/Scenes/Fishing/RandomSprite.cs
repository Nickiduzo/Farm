using UnityEngine;

namespace Fishing
{
    public class RandomSprite : MonoBehaviour
    {
        [SerializeField] private FishBody[] _bodies;
        private FishBody _lastSelectedSprite;

        public void SetUpSpriteOrder(int sortOrderIndex)
        {
            FishBody selectedSprite = GetRandomSprite();
            if (selectedSprite == _lastSelectedSprite)
            {
                selectedSprite.ChangeSpriteSortOrder(10);
            }
            else
            {
                for (int i = 0; i < _bodies.Length; i++)
                {
                    _bodies[i].ChangeSpriteSortOrder(sortOrderIndex);
                }
            }

            _lastSelectedSprite = selectedSprite;
        }

        public FishBody GetRandomSprite()
            => _bodies[Random.Range(0, _bodies.Length)];
    }
}