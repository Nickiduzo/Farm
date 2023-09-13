using CowScene.Spawners.Pools;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace CowScene.Spawners
{
    public class MilkShopSpawner : MonoBehaviour
    {
        public event Action<CowMilkShop> OnShopSpawn;

        [Header("Positions")]
        [SerializeField] private Transform _shopSpawnPoint;
        [SerializeField] private Transform _shopDestinationPoint;
        [SerializeField] private Transform _shopEndPoint;
        [Header("Pool")]
        [SerializeField] private MilkShopPool _pool;

        //spawns shop
        public void SpawnShop(SoundSystem soundSystem, FxSystem fxSystem, InputSystem inputSystem)
        {
            CowMilkShop cowMilkShop = _pool.Pool.GetFreeElement();
            cowMilkShop.transform.position = _shopSpawnPoint.position;

            cowMilkShop.Construct(_shopDestinationPoint.position, _shopEndPoint.position, soundSystem, fxSystem, inputSystem);
            OnShopSpawn?.Invoke(cowMilkShop);
        }
    }
}

