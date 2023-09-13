using Bee.Config;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using AwesomeTools;
using System.Collections.Generic;
using UnityEngine;

namespace Bee.Spawners
{
    public class HiveSpawner : MonoBehaviour
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private BeeLevelConfig _config;
        [SerializeField] private Transform[] _spawnPoints;

        private int _maxCount => _config.Hives.Length;


        //Spawns hives
        public List<IHive> SpawnHives(SoundSystem soundSystem, FxSystem fxSystem)
        {
            List<IHive> hives = new List<IHive>();

            for (int i = 0; i < _maxCount; i++)
            {
                var hive = Instantiate(_config.Hives[i], _spawnPoints[i].position, Quaternion.identity);
                hive.Construct(soundSystem, fxSystem);
                hive.GetComponentInChildren<DragAndDrop>()
                    .Construct(_inputSystem);
                hives.Add(hive);
            }
            return hives;
        }


    }
}