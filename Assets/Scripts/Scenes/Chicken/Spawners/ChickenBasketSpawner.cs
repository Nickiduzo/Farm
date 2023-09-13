using System;
using ChickenScene.Entities;
using ChickenScene.Pools;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;

namespace ChickenScene.Spawners
{
    public class ChickenBasketSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _basketSpawnPosition;
        [SerializeField] private Transform _basketDestinationPoint;
        [SerializeField] private ChickenBasketPool _chickenBasketPool;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;

        public event Action<ChickenBasket> OnSpawn;

        // get basket from pool and set basket to spawn point, aslo invoke action [OnSpawn] in which we pass "ChickenBasket"
        public void SpawnChickenBasket()
        {
            ChickenBasket basket = _chickenBasketPool.Pool.GetFreeElement();
            basket.transform.position = _basketSpawnPosition.position;
            basket.Construct(_basketDestinationPoint.position,
                _basketSpawnPosition.position,
                _soundSystem,_inputSystem);

            OnSpawn?.Invoke(basket);
        }
    }
}