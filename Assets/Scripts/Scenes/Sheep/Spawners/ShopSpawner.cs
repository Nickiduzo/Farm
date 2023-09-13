using AwesomeTools.Inputs;
using Sheep.Spawners.Pools;
using AwesomeTools.Sound;
using UnityEngine;

namespace Sheep.Spawners
{
    public class ShopSpawner : MonoBehaviour
    {
        [SerializeField] ArrowController _arrowController;
        [SerializeField] private FurStorage _furStorage;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private ShopPool _pool;

        //Spawns shop
        public Shop SpawnShop(SoundSystem soundSystem, FxSystem fxSystem)
        {
            Shop spawnedShop = _pool.Pool.GetFreeElement();
            spawnedShop.transform.position = _spawnPoint.position;
            spawnedShop.Construct(_furStorage, _startPoint.position);

            var storeContainer = spawnedShop.GetComponentInChildren<StoreContainer>();
            storeContainer.Construct(soundSystem, _inputSystem);

            return spawnedShop;
        }

    }
}