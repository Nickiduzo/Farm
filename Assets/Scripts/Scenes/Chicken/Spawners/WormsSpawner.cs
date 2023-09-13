using System.Collections.Generic;
using System.Collections;
using ChickenScene.Entities;
using ChickenScene.Pools;
using AwesomeTools.Inputs;
using UnityEngine;
using UsefulComponents;
using Random = UnityEngine.Random;
using AwesomeTools.Sound;

namespace ChickenScene.Spawners
{
    public class WormsSpawner : MonoBehaviour
    {
        [SerializeField] private List<Transform> _wormsSpawnPositions;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private WormsChickenPool _pool;
        [SerializeField] private Transform _poolTransform;
        [SerializeField] private SoundSystem _soundSystem;
        
        private int _spawnedWormsAmount;
        public bool _firstWormDestroyed { private get; set; }
        public int WormsToSpawn => _pool.WormsToSpawn;

        // check how many worms were spawned
        public void SpawnWorm()
        {
            if (_spawnedWormsAmount < WormsToSpawn)
            {
                GetWorm();
                _spawnedWormsAmount++;   
            }
        }
        
        // get worm from pool and set it on selected spawn point, also check whether this is first worm created
        private WormChicken GetWorm()
        {
            WormChicken wormChicken = _pool.Pool.GetFreeElement();
            var start = GetRandomSpawnPosition();
            wormChicken.Construct(start, _inputSystem, _soundSystem, _wormsSpawnPositions);

            if(_firstWormDestroyed == false)
            {
                StartCoroutine(CalculatingHintPosition());
            }
            else
            {
                StopCoroutine(CalculatingHintPosition());
            }

            return wormChicken;
        }

        // hint follows position of worm (update hint position every 0.5 seconds)
        private IEnumerator CalculatingHintPosition()
        {
            while (_firstWormDestroyed == false)
            {
                foreach (Transform child in _poolTransform)
                {
                    if (child.gameObject.activeInHierarchy && child.CompareTag("Worms"))
                    {
                        HintSystem.Instance.ShowPointerHint(child.position);
                    }
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        
        // select random spawn point
        private Vector3 GetRandomSpawnPosition() 
            => _wormsSpawnPositions[Random.Range(0,_wormsSpawnPositions.Count)].position;
    }
}
