using System;
using UnityEngine;

namespace SunflowerScene
{
    [Serializable]
    public class PlacementByOffset
    {
        [SerializeField] private Transform[] _positions;
        [SerializeField] private Vector3 _offset = new(0, .1f);
        private Vector3 _currentOffset = Vector3.zero;
        private int _index;

        // Retrieves the current position and updates to the next position
        public Vector3 GetPosition()
        {
            var pos = _positions[_index].position + _currentOffset;
            Next();

            return pos;
        }


        // Moves to the next position in the array or resets to the first position and applies the offset if necessary
        private void Next()
        {
            if (_index + 1 >= _positions.Length)
            {
                _index = 0;
                _currentOffset += _offset;
                return;
            }

            _index++;
        }

    }
}