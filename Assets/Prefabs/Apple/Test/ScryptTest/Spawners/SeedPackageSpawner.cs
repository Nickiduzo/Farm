using AwesomeTools.Inputs;
using UnityEngine;

namespace Apple.Spawners
{
    public class SeedPackageSpawner : MonoBehaviour
    {
        [SerializeField] private InputSystem _input;
        [SerializeField] private SeedPackagePool _pool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;


        public SeedPackage SpawnPackage()
        {
            SeedPackage package = _pool.Pool.GetFreeElement();
            package.transform.position = _spawnPoint.position;
            package.Construct(_destinationPoint.position, _spawnPoint.position, _input);
            return package;
        }
    }
}