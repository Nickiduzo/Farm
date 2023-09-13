using AwesomeTools.Sound;
using UnityEngine;

namespace Carrot.Spawners
{
    public class CarrotBasketSpawner : MonoBehaviour
    {
        private const float X_POS = 0.18f;
        private const float Y_POS = 0.18f;
        
        [SerializeField] private Camera _camera;
        [SerializeField] ArrowController _arrowController;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private Transform _basketCarrotsPositions;
        [SerializeField] private CarrotBasketPool _pool;
        [SerializeField] private float _delay;
        

        // get basket from pool
        public CollectionArea SpawnBasket(FxSystem fxSystem, SoundSystem soundSystem)
        {
            CollectionArea basket = _pool.Pool.GetFreeElement();
            CalculateDestinationPoint();
            basket.transform.position = _spawnPoint.position;
            basket.Construct(_spawnPoint.position, _destinationPoint.position, fxSystem, soundSystem, _arrowController, _delay);
            return basket;
        }

        // set destination point, point for products and arrow position above the basket
        private void CalculateDestinationPoint()
        {
            Vector3 arrowControllerYOffset = new Vector3(0, 2.4f, 0);
            _basketCarrotsPositions.position = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, (Y_POS + 0.05f) * Screen.height, 1));
            _destinationPoint.position = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _arrowController.transform.position = _destinationPoint.position + arrowControllerYOffset;
        }

        // turn off arrow above basket when we first pick up carrot
        public void DisableBasketArrowHint()
            => _arrowController.HideArrow();
    }
}