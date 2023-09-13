using UnityEngine;
using AwesomeTools.Sound;

namespace SunflowerScene
{
    public class SunflowerBasketSpawner : MonoBehaviour
    {   
        private const float X_POS = 0.14f;
        private const float Y_POS = 0.20f;
        
        [SerializeField] Camera _camera;
        [SerializeField] ArrowController _arrowController;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private Transform _basketStorePoints;
        [SerializeField] private SeedBasketPool _pool;
        [SerializeField] private float _delay;

        // Spawns a basket prefab at the designated spawn point, with the specified FX system and sound system.
        public CollectionArea SpawnBasket(FxSystem fxSystem, SoundSystem soundSystem)
        {
            CalculateDestinationPoint();
            CollectionArea basket = _pool.Pool.GetFreeElement();
            basket.transform.position = _spawnPoint.position;
            basket.Construct(_spawnPoint.position, _destinationPoint.position, fxSystem, soundSystem, _arrowController, _delay);
            return basket;
        }

        // Calculates the destination point based on the screen position and updates related positions and transforms.
        private void CalculateDestinationPoint()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _destinationPoint.position = destination;
            _basketStorePoints.position = destination;
            _arrowController.transform.position = destination;
        }
    }
}