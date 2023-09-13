using Unity.VisualScripting;
using UnityEngine;

namespace Sheep.Spawners
{
    //Uses to merge spawners callbacks
    public class SheepLevelMediator : BaseLevelMediator, IStartScenable
    {
        [Header("Sheep Mediator")]
        [SerializeField] private SheepSpawner _sheepSpawner;
        [SerializeField] private ShopSpawner _shopSpawner;
        [SerializeField] private TrimmerSpawner _trimmerSpawner;
        [SerializeField] private FurBasketSpawner _basketSpawner;
        [SerializeField] private ComposedFurSpawner _composedFurSpawner;
        [SerializeField] private SheepLevelConfig _sheepLevelConfig;
        private bool _isFirstSheep = true;
        private bool _isFirstHint = true;
        private CollectionArea basket;

        public void StartScene()
        {
            _sheepSpawner.OnSheepSpawned += SheepSpawned;
            _trimmerSpawner.OnTrimmerSpawned += TrimmerSpawned;
            _composedFurSpawner.OnAllFurComposed += AllFurComposed;
            _sheepSpawner.OnSheepEnded += SheepEnded;
            _sheepSpawner.OnSheepEnded += _actorUI.HideProgressBar;

            _trimmerSpawner.SpawnTrimmer(_fxSystem);
            _sheepSpawner.SpawnSheep();
            _soundSystem.InitLevelMusic();            
        }
        // It subscribes to events
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _sheepSpawner.OnSheepSpawned += SheepSpawned;
            _trimmerSpawner.OnTrimmerSpawned += TrimmerSpawned;
            _composedFurSpawner.OnAllFurComposed += AllFurComposed;
            _sheepSpawner.OnSheepEnded += SheepEnded;
            _sheepSpawner.OnSheepEnded += _actorUI.HideProgressBar;

            _trimmerSpawner.SpawnTrimmer(_fxSystem);
            _sheepSpawner.SpawnSheep();
            _soundSystem.InitLevelMusic();            
        }

        // It unsubscribes from events
        private void OnDestroy()
        {
            _sheepSpawner.OnSheepSpawned -= SheepSpawned;
            _trimmerSpawner.OnTrimmerSpawned -= TrimmerSpawned;
            _composedFurSpawner.OnAllFurComposed -= AllFurComposed;
            _sheepSpawner.OnSheepEnded -= SheepEnded;
            if(basket != null)
                basket.GetComponent<ComposedFurTriggerObserver>().OnTriggerEnter -= basket.StoreObj;
        }

        // Handles the event when the sheep's lifecycle ends.
        private void SheepEnded()
        {
            // Spawn a shop and attach event handlers
            Shop spawnedShop = _shopSpawner.SpawnShop(_soundSystem, _fxSystem);
            spawnedShop.OnArrived += _composedFurSpawner.Construct;

            // Get the store container from the spawned shop and attach event handlers
            var storeContainer = spawnedShop.GetComponentInChildren<StoreContainer>();
            storeContainer.OnComposedFurReady += _composedFurSpawner.SpawnFur;
            _isFirstHint = false;
        }

        // Handles the event when all fur is composed.
        private void AllFurComposed()
        {
            // Spawn a basket and initialize its properties
            basket = _basketSpawner.SpawnBasket(_fxSystem, _soundSystem);
            _actorUI.InitWinInvoker(basket);
            basket.STORED_MAX_COUNT = _sheepLevelConfig.ComposedFurToSpawn;
            basket.SetSortingIndex();
            basket.AddComponent<ComposedFurTriggerObserver>().OnTriggerEnter += basket.StoreObj;

            ShowHint();
        }

        // Handles the event when a trimmer is spawned and attaches an event handler for the sheep's lifecycle end.
        private void TrimmerSpawned(Trimmer trimmer)
        {
            _sheepSpawner.OnSheepEnded += trimmer.EndLifeCycle;
        }

        // Handles the event when a sheep is spawned.
        private void SheepSpawned(Sheep sheep)
        {
            if (_isFirstSheep)
            {
                // Attach event handlers for the first sheep
                sheep.OnArrived += _trimmerSpawner.SpawnedTrimmer.MoveToStartPoint;
                sheep.OnArrived += ShowHint;
                _isFirstSheep = false;
            }

            // Attach event handlers for subsequent sheep
            sheep.OnArrived += _trimmerSpawner.SpawnedTrimmer.MakeInteractable;
            sheep.OnWholeFurTrimmed += _trimmerSpawner.SpawnedTrimmer.MakeNonInteractable;
        }

        // Shows the appropriate hint based on the game state.
        private void ShowHint()
        {
            if (_isFirstHint)
            {
                Debug.Log("FirstSheep");
                ActivateHint(GameObject.Find("TrimmerStartPoint").transform.position, GameObject.Find("SheepStartPoint").transform.position);
            }
            else
            {
                ActivateHint(GameObject.Find("FurDestination").transform.position, GameObject.Find("BasketDestinationPoint").transform.position);
            }
        }
    }
}