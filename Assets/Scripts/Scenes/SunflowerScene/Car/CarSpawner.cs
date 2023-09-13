using UnityEngine;

namespace SunflowerScene
{
    public class CarSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _endPoint;
        [SerializeField] private Transform _rotatePoint;
        [SerializeField] private CarPool _carPool;


        // Spawns a car object and initializes its position and construction parameters.
        public Car SpawnCar()
        {
            Car car = _carPool.Pool.GetFreeElement();
            car.transform.position = _spawnPoint.position;
            car.Construct(_endPoint, _rotatePoint);
            return car;
        }
    }
}