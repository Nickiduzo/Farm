using System;
using UnityEngine;

namespace SunflowerScene
{
    public sealed class ConsistentPlantingInHole : MonoBehaviour, IPointable, ITarget
    {
        [SerializeField] private SunflowerHole[] _holes;
        [SerializeField] private Vector3 _pointOffset = new(0, 1, 0);
        private int _holeIndex;
        private int _rotateIndex => _holes.Length / 2;
        public SunflowerHole[] Holes => _holes;
        public bool LastOnWay => _rotateIndex == _holeIndex;

        public SunflowerHole CurrentHole
        {
            get
            {
                if (_holeIndex >= _holes.Length)
                {
                    Debug.LogError("Index is out of bounds ");
                }

                return _holes[_holeIndex];
            }
        }

        // Moves to the next hole in the sequence if there is one.
        public void Next()
        {
            if (_holeIndex + 1 < _holes.Length)
            {
                _holeIndex++;
            }
        }

        // Unlocks the current hole if it is not already planted.
        public void UnLockCurrentHole()
        {
            if (CurrentHole.Planted == false)
            {
                CurrentHole.SetAvailableStore(true);
            }
        }

        // Retrieves the position of the point associated with the current hole.
        public Vector3 GetPointPosition()
        {
            return CurrentHole.transform.position + _pointOffset;
        }

        // Retrieves the position of the car stop point for the current hole.
        public Vector3 Position()
        {
            return CurrentHole.CarStopPoint;
        }
    }
}