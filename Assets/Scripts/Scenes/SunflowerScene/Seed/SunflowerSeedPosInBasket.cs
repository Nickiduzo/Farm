using UnityEngine;

namespace SunflowerScene
{
    public class SunflowerSeedPosInBasket : MonoBehaviour
    {
        [SerializeField] private Transform[] nextSeedPos;
        private int _num = -1;
        private float _offsetY = 0f;

        public Vector2 GetPos()
        {
            return nextSeedPos[GenNumPos()].position + (Vector3.up * _offsetY);
        }
        private int GenNumPos()
        {
            _num++;
            if (_num >= 7)
            {
                _offsetY += -.15f;  
                _num = 0;
            }
            return _num;
        }
    }
}