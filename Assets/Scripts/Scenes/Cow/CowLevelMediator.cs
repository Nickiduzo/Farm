using CowScene.Spawners;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

namespace CowScene
{
    public class CowLevelMediator : BaseLevelMediator, IStartScenable
    {
        [Header("Spawners")]
        [SerializeField] private HaySpawner _haySpawner;
        [SerializeField] private CowSpawner _cowSpawner;
        [SerializeField] private MilkJarSpawner _jarSpawner;
        [SerializeField] private MilkShopSpawner _shopSpawner;
        [SerializeField] private MilkBasketSpawner _milkBasketSpawner;
        [SerializeField] private MilkBottleSpawner _bottleSpawner;
        [SerializeField] private BottleBasketSpawner _bottleBasketSpawner;
        [Header("Controllers")]
        [SerializeField] private MilkShopController _milkShopController;
        [SerializeField] private CowController _cowController;
        [SerializeField] private HayController _hayController;
        [Header("Config")]
        [SerializeField] private CowLevelConfig _config;

        private CollectionArea _milkBasket;
        private CollectionArea basket;

        public void StartScene()
        {
            _soundSystem.InitLevelMusic();
            _cowController.CowFullyFed += CowFullyFed;
            _haySpawner.SpawnHay(_config.HayCount);

            Cow cow = _cowSpawner.SpawnCow(_soundSystem, _fxSystem);
            _cowController.InitiateCowController(_config.BottleCount, cow);
            _actorUI.InitProgressBar(cow);
            cow.OnArrived += OnCowArrived;
            _hayController.OnAnyHayDrag += _cowController.Cow.PreparingToEat;
        }
        // It subscribes to events and spawn cow
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _soundSystem.InitLevelMusic();
            _cowController.CowFullyFed += CowFullyFed;
            _haySpawner.SpawnHay(_config.HayCount);

            Cow cow = _cowSpawner.SpawnCow(_soundSystem, _fxSystem);
            _cowController.InitiateCowController(_config.BottleCount, cow);
            _actorUI.InitProgressBar(cow);
            cow.OnArrived += OnCowArrived;
            _hayController.OnAnyHayDrag += _cowController.Cow.PreparingToEat;
        }

        private void OnDestroy()
        {
            if(basket != null)
                basket.AddComponent<BottleBasketTriggerObserver>().OnTriggerEnter -= basket.StoreObj;
        }

        // Handles the event when the cow is fully fed. It sets up the next steps including spawning jars, showing hints, and subscribing to the event for when the cow is fully milked.
        private void CowFullyFed()
        {
            _hayController.OnAnyHayDrag -= _cowController.Cow.PreparingToEat;
            _cowController.CowFullyFed -= CowFullyFed;
            _jarSpawner.SpawnJars(_config.JarCount);
            ActivateHint(_haySpawner.HintPointPosition, _cowController.Cow.MilkingHintPoint);
            _cowController.CowFullyMilked += CowFullyMilked;
        }

        // Handles the event when the cow is fully milked. It hides the progress bar
        private void CowFullyMilked()
        {
            _actorUI.HideProgressBar();
            _cowController.CowFullyMilked -= CowFullyMilked;
            _cowController.CowWalkedAway += SpawnMilkJarBasket;
        }

        // Spawns the milk jar basket
        private void SpawnMilkJarBasket()
        {
            _cowController.CowWalkedAway -= SpawnMilkJarBasket;
            _milkBasket = _milkBasketSpawner.SpawnBasket(_soundSystem, _fxSystem);
            _milkBasket.STORED_MAX_COUNT = _config.BottleCount;
            _milkBasket.SetSortingIndex();
            _milkBasket.AddComponent<MilkBasketTriggerObserver>().OnTriggerEnter += _milkBasket.StoreObj;

            DOVirtual.DelayedCall(1f, () =>
            {
                OnBasketArrived();
            });
        }

        // Handles the event when the milk jar basket arrives and shows hints for the next step.
        private void OnBasketArrived()
        {
            _milkBasket.OnAllProductsStored += OnAllJarsStored;
            ActivateHint(_haySpawner.OtherHintPointPosition, _milkBasketSpawner.HintPointPosition);
            _jarSpawner.MakeAllJarsInteractable();
        }

        // Handles the event when all jars are stored. It schedules the next steps including destroying old points, spawning the shop
        private void OnAllJarsStored()
        {
            DOVirtual.DelayedCall(1f, () =>
            {
                _milkBasketSpawner.DestroyOldPoints();
                _milkBasket.OnAllProductsStored -= OnAllJarsStored;
                _milkBasket.GetComponent<MilkBasketTriggerObserver>().OnTriggerEnter -= _milkBasket.StoreObj;
                _shopSpawner.OnShopSpawn += ShopSpawned;
                _shopSpawner.SpawnShop(_soundSystem, _fxSystem, _inputSystem);
            });
        }

        // Handles the event when the shop is spawned and activating all bottles.
        private void ShopSpawned(CowMilkShop _shop)
        {
            _milkShopController.ShopSpawned(_shop);
            _milkShopController.SpawnBottle += SpawnBottle;
            _milkShopController.AllBottlesFilled += SpawnBottleBasket;
        }

        // Spawns a milk bottle
        private void SpawnBottle()
        {
            _bottleSpawner.OnBottleSpawn += OnBottleSpawn;
            _bottleSpawner.SpawnBottle();
        }

        // Spawns a bottle basket and activates all bottles.
        private void SpawnBottleBasket()
        {
            basket = _bottleBasketSpawner.SpawnBasket(_soundSystem, _fxSystem);

            basket.STORED_MAX_COUNT = _config.BottleCount;
            basket.SetSortingIndex();
            basket.AddComponent<BottleBasketTriggerObserver>().OnTriggerEnter += basket.StoreObj;
            _actorUI.InitWinInvoker(basket);
            _milkShopController.ActivateAllBottles();
        }

        // Called when the cow arrives. Shows a hint for feeding the cow.
        private void OnCowArrived()
            => ActivateHint(_haySpawner.HintPointPosition, _cowController.Cow.FeedingHintPoint);

        // Called when a milk bottle is spawned. Notifies the milk shop controller about the spawned bottle.
        private void OnBottleSpawn(MilkBottle bottle)
            => _milkShopController.BottleSpawned(bottle);
    }
}