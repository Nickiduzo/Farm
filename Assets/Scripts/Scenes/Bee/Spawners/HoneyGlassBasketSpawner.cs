using UnityEngine;
using AwesomeTools.Sound;

namespace Bee.Spawners
{
    public class HoneyGlassBasketSpawner : MonoBehaviour
    {
        private const float X_POS = 0.8f;
        private const float Y_POS = 0.16f;
        private const float NEW_SCALE_X = 1.31f;
        private const float NEW_SCALE_Y = 1f;

        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _basketStorePoints;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private ArrowController _arrowController;
        [SerializeField] private HoneyGlassBasketPool _pool;
        [SerializeField] private float _delay;

        // Spawns a basket and sets up its properties and positions.
        public CollectionArea SpawnBasket(FxSystem fxSystem, SoundSystem soundSystem)
        {
            CalculateDestinationPoint();
            CollectionArea basket = _pool.Pool.GetFreeElement();
            basket.transform.localScale = new Vector3(NEW_SCALE_X, NEW_SCALE_Y, 1);
            basket.transform.position = _spawnPoint.position;
            basket.Construct(_spawnPoint.position, _destinationPoint.position, fxSystem, SoundSystemUser.Instance, _arrowController, _delay);
            return basket;
        }

        // Calculates the destination point based on the screen position and sets positions for basket store points and arrow controller.
        private void CalculateDestinationPoint()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _destinationPoint.position = destination;
            _basketStorePoints.position = destination;
            _arrowController.transform.position = destination;
        }
        
    }
}