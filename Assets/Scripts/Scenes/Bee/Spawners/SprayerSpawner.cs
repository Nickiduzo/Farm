using Bee.Config;
using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using AwesomeTools;
using UnityEngine;
using UsefulComponents;

namespace Bee.Spawners
{
    public class SprayerSpawner : MonoBehaviour
    {
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private BeeLevelConfig _config;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;

        public Vector3 HintPoint => _destinationPoint.position;


        //spawn sprayer
        public Sprayer SpawnSprayer(SoundSystem soundSystem, FxSystem fxSystem)
        {
            Sprayer sprayer = Instantiate(_config.Sprayer, _spawnPoint.position, Quaternion.identity);

            sprayer.Construct(soundSystem, fxSystem);
            sprayer.GetComponent<DragAndDrop>()
                .Construct(_inputSystem);
            sprayer.GetComponent<MoveToDestinationOnDragEnd>()
                .Construct(_destinationPoint.position);
            sprayer.GetComponent<MoveStartDestination>()
                .Construct(_destinationPoint.position, _spawnPoint.position);

            return sprayer;
        }
    }
}