using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunflowerScene
{
    public class PestSpawner : MonoBehaviour
    {
        [SerializeField] private PestPool _pestPool;
        [SerializeField] private float _spawnDelay;

        [SerializeField] private PestPathContainer _path;

        private readonly List<Pest> _spawnedPest = new();

        private bool ContinueSpawn;
        private int _indexMinus = 0;
        private int _indexPlus = 25;
        private int _pestNumber = 0;
        private int _sortingLayerID;

        public event Action<Pest> PestSpawned;

        //Init pestPool and constructing path 
        public void Construct(int countInPool)
        {
            _sortingLayerID = SortingLayer.NameToID("Product");
            _pestPool.Init(countInPool);
            _path.Construct();
        }

        // Starts the spawning process by enabling continuous spawning and starting the coroutine.
        public void StartSpawn()
        {
            ContinueSpawn = true;
            StartCoroutine(Spawning());
        }

        // Stops the spawning process by disabling continuous spawning.
        public void StopSpawn()
        {
            ContinueSpawn = false;
        }

        // Coroutine for spawning pests at a specified spawn delay.
        private IEnumerator Spawning()
        {
            while (CanContinueSpawn())
            {
                var pest = _pestPool.Pool.GetFreeElement();
                var path = _path.GetNextPath();
                pest.Construct(path);
                
                _pestNumber += 1;
                if (_pestNumber % 2 == 0)
                {
                    _indexPlus += 6;
                    pest.StartChangingSortingOrder(_indexPlus);
                }
                else
                {
                    _indexMinus += 6;
                    pest.StartChangingSortingOrder(_indexMinus);
                }

                _spawnedPest.Add(pest);
                PestSpawned?.Invoke(pest);
                yield return new WaitForSeconds(_spawnDelay);
            }
        }

        // Checks if the spawning can continue based on conditions.
        private bool CanContinueSpawn()
        {
            if (ContinueSpawn == false)
            {
                return false;
            }

            if (_pestPool.Pool.HasFreeElement() == false)
            {
                return false;
            }

            return ContinueSpawn;
        }


        // Returns an array of spawned pests.
        public Pest[] GetObjects()
        {
            return _spawnedPest.ToArray();
        }
    }
}