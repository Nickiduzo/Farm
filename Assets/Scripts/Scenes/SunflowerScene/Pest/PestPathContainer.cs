using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SunflowerScene
{
    [Serializable]
    public class PestPathContainer
    {
        [SerializeField] private List<Path> _paths;

        private Queue<Path> _queuePath;


        // Constructs the PathManager by initializing the queue of paths.
        public void Construct()
        {
            _queuePath = new Queue<Path>(_paths);
        }

        // Retrieves the next available path
        public Path GetNextPath()
        {
            if (_queuePath.Count == 0)
            {
                var index = Random.Range(0, _paths.Count);
                return _paths[index];
            }

            return _queuePath.Dequeue();
        }
    }
}