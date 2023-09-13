using Sheep.Spawners.Pools;
using AwesomeTools.Sound;
using UnityEngine;

namespace Sheep.Spawners
{
    public class FurBasketSpawner : MonoBehaviour
    {
        [SerializeField] private float _delay;
        [SerializeField] Vector3 _arrowOffset;
        [SerializeField] Vector3 _basketFrontLocalScale;
        [SerializeField] Vector3 _basketFrontLocalPosition;
        [SerializeField] Vector2 _basketScale;
        [SerializeField] ArrowController _arrowController;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private FurBasketPool _pool;

        // Returns the spawned basket
        public CollectionArea SpawnBasket(FxSystem fxSystem, SoundSystem soundSystem)
        {
            CollectionArea basket = _pool.Pool.GetFreeElement();
            SetToBasket(basket);            
            _arrowController.transform.position = _destinationPoint.position + _arrowOffset;
            basket.Construct(_spawnPoint.position, _destinationPoint.position, fxSystem, soundSystem, _arrowController, _delay);
            return basket;
        }

        // Sets the position and scales of the basket prefab and its child objects
        private void SetToBasket(CollectionArea basket)
        {
            basket.transform.position = _spawnPoint.position;
            Transform basketBackTrans = basket.transform.Find("BasketBack");
            basketBackTrans.localScale = _basketScale;
            Transform basketFrontTrans = basket.transform.GetChild(0);
            basketFrontTrans.localScale = _basketFrontLocalScale;
            basketFrontTrans.localPosition = _basketFrontLocalPosition;
        }
    }
}