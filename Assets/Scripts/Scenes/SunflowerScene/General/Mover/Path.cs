using System;
using UnityEngine;

namespace SunflowerScene
{
    [Serializable]
    public class Path
    {
        [SerializeField] private Transform[] _points;
        [SerializeField] private VerticalPosition _verticalPosition;
        public VerticalPosition VerticalPosition => _verticalPosition;

        // Retrieves the positions of the points in the path
        public Vector3[] GetPath()
        {
            var positions = new Vector3[_points.Length];
            for (var index = 0; index < _points.Length; index++)
            {
                positions[index] = _points[index].position;
            }

            return positions;
        }

        // Retrieves the start position of the path
        public Vector3 GetStartPosition()
        {
            var path = GetPath();
            if (path.Length == 0)
            {
                Debug.LogError("Path has no positions");
            }

            return path[0];
        }
    }
}