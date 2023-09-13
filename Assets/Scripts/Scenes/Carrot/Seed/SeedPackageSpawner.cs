using AwesomeTools.Inputs;
using UnityEngine;

namespace Carrot.Spawners
{
    public class SeedPackageSpawner : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputSystem _input;
        [SerializeField] private SeedPackagePool _pool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;


        // get [SeedPackage] from pool, set spawn and destination point, invoke "Construct"  
        public SeedPackage SpawnPackage()
        {
            SeedPackage package = _pool.Pool.GetFreeElement();
            package.transform.position = _spawnPoint.position;
            _destinationPoint.position = _camera.ScreenToWorldPoint(new Vector3(0.8f * Screen.width, 0.25f* Screen.height, 1));
            package.Construct(_destinationPoint.position, _spawnPoint.position, _input);
            return package;
        }
    }
}