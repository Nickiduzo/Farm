using AwesomeTools.Sound;
using Tomato.Spawners.Pools;
using UnityEngine;

namespace Tomato.Spawners
{
    public class TomatoBasketSpawner : MonoBehaviour
    {
        private const float X_POS = 0.15f;
        private const float Y_POS = 0.14f;
        private const float NEW_SCALE = .7f;
        private const string COLLECT_POINT_NAME = "TopBasketPoint";

        [SerializeField] private Vector3 _basketFront_localScale;
        [SerializeField] private Vector3 _basket_localPositition;
        [SerializeField] private Vector2 _basket_colliderOffset;
        [SerializeField] private Vector2 _basket_colliderSize;
        [SerializeField] private Camera _camera;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _basketStorePoints;
        [SerializeField] private TomatoBasketPool _pool;
        [SerializeField] private ArrowController _arrowController;
        [SerializeField] private float _delay;
        [SerializeField] private Sprite _basketSprite;
        [SerializeField] private Sprite _basketFrontSprite;
        private Vector3 _destinationPositition;

        /// <summary>
        /// Вводимо систему ефектів та звуку - викликає кошик для томатів [basket]
        /// </summary>        
        public CollectionArea SpawnBasket(FxSystem fxSystem, SoundSystem soundSystem)
        {
            Vector3 topBasketPointLocalPositition = new Vector3(0, 1, 0);
            CalculateDestinationPoint();
            CollectionArea basket = _pool.Pool.GetFreeElement();
            basket.transform.Find("BasketBack").localScale = new Vector3(NEW_SCALE, NEW_SCALE, 0);
            SetValueForComponents(basket);
            SetBasketFrontSprite(basket);
            basket.transform.position = _spawnPoint.position;
            basket.transform.Find(COLLECT_POINT_NAME).localPosition = topBasketPointLocalPositition;
            basket.Construct(_spawnPoint.position, _destinationPositition, FxSystem.Instance, SoundSystemUser.Instance, _arrowController, _delay);
            return basket;
        }

        /// <summary>
        /// Вводимо кошик [basket]- присвоюємо значення до компонентів кошику
        /// </summary>        
        private void SetValueForComponents(CollectionArea basket)
        {
            Transform basketBackTrans = basket.transform.Find("BasketBack");
            basketBackTrans.GetComponent<SpriteRenderer>().sprite = _basketSprite;
            basketBackTrans.GetComponent<SpriteRenderer>().sortingOrder = 10;
            basket.GetComponent<BoxCollider2D>().offset = _basket_colliderOffset;
            basket.GetComponent<BoxCollider2D>().size = _basket_colliderSize;
        }

        /// <summary>
        /// Вводимо банку [basket]- присвоюємо значення до лицевої частини кошику
        /// </summary>        
        private void SetBasketFrontSprite(CollectionArea basket)
        {
            Transform basketFrontTrans = basket.transform.GetChild(0);
            basketFrontTrans.GetComponent<SpriteRenderer>().sprite = _basketFrontSprite;
            basketFrontTrans.GetComponent<SpriteRenderer>().sortingOrder = 25;
            basketFrontTrans.localScale = _basketFront_localScale;
            basketFrontTrans.localPosition = _basket_localPositition;
            basketFrontTrans.localPosition = _basket_localPositition;
        }

        /// <summary>
        /// Рахуємо позицію призначення для кошика, точок складування та підказки
        /// </summary>
        private void CalculateDestinationPoint()
        {
            Vector3 _basketStorePointsYOffset = new Vector3(-.42f, -.5f, 0);
            Vector3 arrowControllerYOffset = new Vector3(0, 3f, 0);
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
            _destinationPositition = destination;
            _basketStorePoints.position = destination + _basketStorePointsYOffset;
            _arrowController.transform.position = destination + arrowControllerYOffset;
        }
    }
}