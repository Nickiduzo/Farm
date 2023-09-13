using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using AwesomeTools;
using UnityEngine;
using UsefulComponents;

namespace Bee.Spawners
{
    public class HoneyGlassSpawner : MonoBehaviour
    {
        [SerializeField] private HoneyGlassPool _pool;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private InputSystem _inputSystem;

        public Vector3 HintPosition => _destinationPoint.position;

        //spawns glass
        public IHoneyGlass SpawnGlass(SoundSystem soundSystem, FxSystem fxSystem)
        {
            HoneyGlass glass = _pool.Pool.GetFreeElement();
            glass.transform.position = _spawnPoint.position;
            glass.Construct(soundSystem, fxSystem);
            glass.GetComponent<DragAndDrop>().Construct(_inputSystem);
            glass.GetComponent<MoveStartDestination>().Construct(_destinationPoint.position, _spawnPoint.position);
            return glass;
        }
    }
}