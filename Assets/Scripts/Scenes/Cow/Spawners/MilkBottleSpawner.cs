using CowScene.Spawners.Pools;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using AwesomeTools;
using System;
using UnityEngine;

namespace CowScene.Spawners
{
    public class MilkBottleSpawner : MonoBehaviour
    {
        public event Action<MilkBottle> OnBottleSpawn;

        [Header("Positions")]
        [SerializeField] private Transform _jumpDestinationPoint;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private Transform _spawnPoint;
        [Header("Systems")]
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [Header("Pool")]
        [SerializeField] private MilkBottlePool _pool;

        //spawns bottle
        public void SpawnBottle()
        {
            MilkBottle bottle = _pool.Pool.GetFreeElement();
            bottle.Construct(_jumpDestinationPoint.position, _destinationPoint.position, _spawnPoint.position, _soundSystem);
            bottle.GetComponent<DragAndDrop>().Construct(_inputSystem);

            OnBottleSpawn?.Invoke(bottle);
        }
    }
}
