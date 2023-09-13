using UnityEngine;
using AwesomeTools.Sound;
using UsefulComponents;

namespace Apple
{
    public class AppleBasketSpawner : MonoBehaviour
    {
        private const float X_POS = 0.13f;
        private const float Y_POS = 0.1f;
        private const float NEW_SCALE = 1.3f;

        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _basketStorePoints;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private ArrowController _arrowController;
        [SerializeField] private AppleBasketPool _pool;
        [SerializeField] private float _delay;

        // Spawn a basket prefab and initialize its properties
        public CollectionArea SpawnBasket(FxSystem fxSystem, SoundSystem soundSystem, Vector3 hintPos)
        {
            CalculateDestinationPoint();
            
            CollectionArea basket = _pool.Pool.GetFreeElement();
            basket.transform.localScale = new Vector3(NEW_SCALE, NEW_SCALE, NEW_SCALE);
            basket.transform.position = _spawnPoint.position;
            basket.Construct(_spawnPoint.position, _destinationPoint.position, FxSystem.Instance, SoundSystemUser.Instance, _arrowController, _delay);

            HintSystem.Instance.ShowPointerHint(hintPos,  _destinationPoint.position);
            return basket;
        }

        // Calculate the destination point based on screen coordinates
        private void CalculateDestinationPoint()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _destinationPoint.position = destination;
            _basketStorePoints.position = destination;
            _arrowController.transform.position = destination;
        }
    }
}