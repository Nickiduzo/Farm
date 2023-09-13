using AwesomeTools.Inputs;
using Tomato.Spawners.Pools;
using UnityEngine;
using AwesomeTools.Sound;

namespace Tomato
{
    public class WormSpawner : MonoBehaviour
    {
        [SerializeField] private WormEdgeCalculator[] _wormEdge;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private WormsPool _pool;

        private bool _firstSpawn = false;

        /// <summary>
        /// Викликає черв'яка
        /// </summary>        
        public Worm SpawnWorm()
        {
            if (!_firstSpawn)
            {
                foreach (var edgeCalculator in _wormEdge)
                {
                    if (edgeCalculator.transform.childCount > 0)
                    {
                        edgeCalculator.StartMonitoring();
                    }
                }
                _firstSpawn = true;
            }

            Worm worm = _pool.Pool.GetFreeElement();
            GetRandomEdgeCalculator()
                .CalculateBounceEdges(worm);
            worm.Construct(_inputSystem, _soundSystem);
            return worm;
        }

        public void StopMonitoring()
        {
            foreach (var edgeCalculator in _wormEdge)
            {
                edgeCalculator.StopMonitoring();
            }
        }

        /// <summary>
        /// Повертає випадкові кордони пересування для черв'яка
        /// </summary>        
        private WormEdgeCalculator GetRandomEdgeCalculator()
        {
            int randomIndex = Random.Range(0, _wormEdge.Length);
            return _wormEdge[randomIndex];
        }
    }
}
