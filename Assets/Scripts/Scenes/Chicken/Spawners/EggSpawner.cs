using System;
using System.Collections;
using System.Collections.Generic;
using ChickenScene.Entities;
using ChickenScene.Pools;
using AwesomeTools.Sound;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

namespace ChickenScene.Spawners
{
    public class EggSpawner : MonoBehaviour
    {
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private List<Transform> _eggSpawnPoints;
        [SerializeField] private EggPool _pool;
        [SerializeField] private float _eggSpawnRate;

        public event Action<Egg> OnSpawn;
        public event Action<int> GetIndex;

        public int EggsToWin => _pool.EggToWin;
        private float _dropPosOffSetY = 3.3f;
        public bool AllEggsStored { get; set; }
        
        private Coroutine _spawnEggRoutine;
        private int _index;
        
        // launch coroutine [SpawnEggsWithDelay()]
        public void StartSpawnEgg()
            => _spawnEggRoutine = StartCoroutine(SpawnEggsWithDelay());
        

        // stop creating eggs (invoke when all eggs are collected)
        public void StopSpawnEgg()
            => StopCoroutine(_spawnEggRoutine);

        // create eggs until all eggs are collected
        private IEnumerator SpawnEggsWithDelay()
        {
            while (!AllEggsStored)
            {
                yield return new WaitForSeconds(_eggSpawnRate); // wait for next egg
                GetEgg();
                yield return new WaitForSeconds(_eggSpawnRate);
            }
        }
        
        // get egg from pool and set spawn point, aslo invoke action [OnSpawn] in which we pass "Egg"
        private Egg GetEgg()
        {
            Egg egg = _pool.Pool.GetFreeElement();
            GetRandomSpawnIndex();
            DOVirtual.DelayedCall(0.9f, () =>
            {
                egg.Construct(GetRandomSpawnPosition(), _soundSystem, _dropPosOffSetY);
                OnSpawn?.Invoke(egg);
            });
            return egg;
        }

        // "_index" is needed to select the same position for _eggSpawnPoints and chicken
        private void GetRandomSpawnIndex()
        {
            _index = Random.Range(0, _eggSpawnPoints.Count);
            GetIndex?.Invoke(_index);
        }

        // select egg spawn point based on "_index"
        private Vector3 GetRandomSpawnPosition()
            => _eggSpawnPoints[_index].position;
    }
}
