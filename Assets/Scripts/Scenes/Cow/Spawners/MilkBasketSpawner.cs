using CowScene.Spawners.Pools;
using AwesomeTools.Sound;
using System;
using UnityEngine;


namespace CowScene.Spawners
{
    public class MilkBasketSpawner : MonoBehaviour
    {
        public event Action OnBasketSpawn;

        [Header("Positions")]
        [SerializeField] private Transform _baksetSpawnPoint;
        [SerializeField] private Transform _baksetDestinationPoint;
        [SerializeField] private GameObject _basketStorePoints;
        [Header("Pool")]
        [SerializeField] private BottleBasketPool _pool;
        [Header("Other")]
        [SerializeField] private float _delay;
        [SerializeField] ArrowController _arrowController;
        private bool _canInvokeOnWin = false;
        public Vector3 HintPointPosition => _baksetDestinationPoint.position;

        // Spawns a basket prefab and returns it
        public CollectionArea SpawnBasket(SoundSystem soundSystem, FxSystem fxSystem)
        {
            CollectionArea jarBasket = _pool.Pool.GetFreeElement();
            OnBasketSpawn?.Invoke();
            jarBasket.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
            jarBasket.transform.position = _baksetSpawnPoint.position;
            jarBasket.Construct(_baksetSpawnPoint.position, _baksetDestinationPoint.position, fxSystem, soundSystem, _arrowController, _delay);
            jarBasket.CanInvokeOnWin(_canInvokeOnWin);

            return jarBasket;
        }

        // Destroys the old basket store points
        public void DestroyOldPoints()
        {
            Destroy(_basketStorePoints);
        }

    }
}
