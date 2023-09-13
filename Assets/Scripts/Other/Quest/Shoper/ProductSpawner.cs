using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Scene;
using System;

namespace Quest
{
    public class ProductSpawner : MonoBehaviour
    {
        [SerializeField] private Product[] _products;
        [SerializeField] private GameObject _basket;
        [SerializeField] private Transform _productsContainer;
        [SerializeField] private SpriteRenderer _basketFrontSprite;
        [SerializeField] private SpriteRenderer _basketbackSprite;
        [SerializeField] private Ease _movingEase;
        [SerializeField] private float _duration;
        [SerializeField] private float _spawnDelayRange;
        [SerializeField] private float _apearHight;
        [SerializeField] private float _jumpPower;

        
        // [_pointOffSetX] - position of point from which products will appear from left to right 
        private float _pointOffSetX = 0.3f; // (the larger the number, the more to the left product will spawn, and vice versa)*
        private float _offSetX = 0.3f; // distance between products
        private float _offSetY = 0.15f;
        private float _productScale = 2.2f;
        private int _numberProductsOnDestination; // number of products already in the basket
        private int _productID; // selected product
        private int _productQty; // quantity of products
        private Vector3 _destinationPointProducts;
        private Transform _basketDestination;
        private Product selectedProduct;
        private bool _complexProduct = false;
        public event Action OnAllProductsStored;

        private Dictionary<string, System.Action> _sceneCodeDictionary;

        private void Awake()
        {
            _sceneCodeDictionary = new Dictionary<string, System.Action>()
            {
                { "FishingScene", SelectedFish },
                { "CarrotScene", SelectedCarrot },
                { "Apple", SelectedApple },
                { "Bee-garden", SelectedBee },
                { "Сhickenscene", SelectedChicken },
                { "CowScene", SelectedCow },
                { "SheepScene", SelectedSheep },
                { "TomatoeScene", SelectedTomato },
                { "SunflowerScene", SelectedSunflower},
            };
        }


        // choose right product depending on type of scene
        public void ProductSelection(SceneData _config, Transform _basketSpawnPoint, Transform _basketDestinationPoint)
        {
            if (_sceneCodeDictionary.TryGetValue(_config.Key, out System.Action selectedMethod))
            {
                selectedMethod?.Invoke();
                if (SceneLoader.IsLevelComplied)
                {
                    SpawnBasket(_basketSpawnPoint, _basketDestinationPoint);
                }
            }
            else
            {
                Debug.LogError("No code defined for SceneData with key: " + _config.Key);
            }
        }

        // create basket and set up necessary parameters
        private void SpawnBasket(Transform _basketSpawnPoint, Transform _basketDestinationPoint)
        {
            _basketDestination = _basketDestinationPoint;
            
            _destinationPointProducts = _basketSpawnPoint.position;
            _destinationPointProducts.y += _offSetY;

            _basket.transform.position = _basketSpawnPoint.position;
            _basket.transform.DOScale(Vector3.one, 1f)
                .OnComplete(() =>
                {                    
                    SpawnProduct();                    
                });
        }

        // create products and move to basket, additional calculations for [_destinationPointProducts]
        private void SpawnProduct()
        {
            _destinationPointProducts.x -= _pointOffSetX * 7f;
            if (_productID >= 0 && _productID < _products.Length)
            {
                selectedProduct = _products[_productID];
                _destinationPointProducts.x += _offSetX;

                for (int i = 0; i < _productQty; i++)
                {
                    Vector3 spawnPosition = _destinationPointProducts + new Vector3(i * _offSetX, 0f, 0f);
                    Product product = Instantiate(selectedProduct, spawnPosition, Quaternion.identity, _productsContainer);
                    if(_complexProduct == true)
                    {
                        product.StartSearchingForChildren();
                        product.GetRandomSprite();
                        product.SetNewScale(_productScale);
                    }

                    product.MoveToBasket(Random.Range(0f, _spawnDelayRange), _apearHight, _duration, _movingEase, _basket)
                        .OnComplete(() =>
                        {
                            AllProductsStored(product);
                        });
                }
            }
        }

        // check if all products are stored, if so, move basket of products in car
        private void AllProductsStored(Product product)
        {
            _numberProductsOnDestination++;
            if (_numberProductsOnDestination == _productQty)
            {
                MoveBasketToCar();
            }
        }

        // move basket of products to car
        private void MoveBasketToCar()
        {
            _basket.transform.DOJump(_basketDestination.position, _jumpPower, 1, 1).OnComplete(() => OnAllProductsStored?.Invoke());
            _basket.transform.SetParent(_basketDestination);
        }
        // set special parameters and values for products, we do this for more accurate position in basket
        private void SelectedFish() { _productID = 0; _productQty = 3; _pointOffSetX = 0.11f; _offSetY = -0.33f; _offSetX = 0.4f;  _complexProduct = true; }
        private void SelectedCarrot() { _productID = 1; _productQty = 5; _pointOffSetX = 0.17f; _offSetY = -0.15f; _offSetX = 0.4f; }
        private void SelectedApple() { _productID = 2; _productQty = 5; _pointOffSetX = 0.14f; _offSetY = -0.3f; }
        private void SelectedBee() { _productID = 3; _productQty = 3; _pointOffSetX = 0.14f; _offSetY = -0.2f; _offSetX = 0.49f; }
        private void SelectedChicken() { _productID = 4; _productQty = 5; _pointOffSetX = 0.14f; _offSetY = -0.3f; }
        private void SelectedCow() { _productID = 5; _productQty = 3; _pointOffSetX = 0.17f; _offSetY = -0.15f; _offSetX = 0.6f; }
        private void SelectedSheep() { _productID = 6; _productQty = 3; _pointOffSetX = 0.14f; _offSetY = -0.3f; _offSetX = 0.45f; }
        private void SelectedTomato() { _productID = 7; _productQty = 5; _pointOffSetX = 0.15f; _offSetY = -0.3f; _offSetX = 0.33f; }
        private void SelectedSunflower() { _productID = 8; _productQty = 5; _pointOffSetX = 0.17f; _offSetY = -0.2f; _offSetX = 0.4f;}
    }
}

