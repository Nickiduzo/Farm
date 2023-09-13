using CowScene.Spawners.Pools;
using AwesomeTools.Sound;
using UnityEngine;

namespace CowScene.Spawners
{
    public class BottleBasketSpawner : MonoBehaviour
    {
        [Header("Positions")]
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;
        [Header("Pool")]
        [SerializeField] private BottleBasketPool _pool;
        [Header("Other")]
        [SerializeField] private float _delay;
        [SerializeField] ArrowController _arrowController;

        //spawns basket
        public CollectionArea SpawnBasket(SoundSystem soundSystem, FxSystem fxSystem)
        {
            CollectionArea bottleBasket = _pool.Pool.GetFreeElement();
            bottleBasket.transform.position = _spawnPoint.position;
            bottleBasket.Construct(_spawnPoint.position, _destinationPoint.position, fxSystem, soundSystem, _arrowController, _delay);
            return bottleBasket;
        }
    }
}


