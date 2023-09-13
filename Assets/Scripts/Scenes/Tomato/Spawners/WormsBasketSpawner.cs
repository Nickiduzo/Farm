using AwesomeTools.Sound;
using Tomato.Spawners.Pools;
using UnityEngine;
using UsefulComponents;

namespace Tomato.Spawners
{
    public class WormsBasketSpawner : MonoBehaviour
    {
        private const float X_POS = 0.1f;
        private const float Y_POS = 0.18f;
        private Vector3 _destinationPoint;
        [SerializeField] private Camera _camera;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private Transform _startPoint;
        [SerializeField] private WormsBasketPool _pool;

        /// <summary>
        /// Викликає банку для черв'яків
        /// </summary>        
        public WormsBasket SpawnBasket()
        {
            WormsBasket basket = _pool.Pool.GetFreeElement();
            basket.transform.position = _startPoint.position;
            CalculateDestinationPoint();
            basket.Construct(_soundSystem);
            basket.GetComponent<MoveStartDestination>()
                .Construct(_destinationPoint, _startPoint.position);
            return basket;
        }

        /// <summary>
        /// Рахує позицію призначення [_destinationPoint]
        /// </summary>
        private void CalculateDestinationPoint()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _destinationPoint = new Vector3(destination.x, destination.y, destination.z);
        }

        /// <summary>
        /// Повертає позицію призначення[_destinationPoint]
        /// </summary>
        public Vector3 GetDestinationPoint()
            => _destinationPoint;
    }
}