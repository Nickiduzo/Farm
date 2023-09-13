using Bee.Config;
using Bee.Spawners;
using DG.Tweening;
using System.Collections;
using UnityEngine;
using UsefulComponents;
using Unity.VisualScripting;

namespace Bee
{
    public class BeeLevelMediator : BaseLevelMediator, IStartScenable
    {
        [Header("Bee Mediator")]
        [SerializeField] private HoneyGlassBasketSpawner _basketSpawner;
        [SerializeField] private GlassFillWithHoneyMediator glassFillWithHoneyMediator;
        [SerializeField] private BeeCameraZoom _camera;
        [SerializeField] private RecyclerSpawner _recyclerSpawner;
        [SerializeField] private HivesController _hivesController;
        [SerializeField] private SprayerSpawner _sprayerSpawner;
        [SerializeField] private BeeSpawner _beeSpawner;
        [SerializeField] private BeeLevelConfig _config;
        [SerializeField] private float _beeSpawnRate;

        private int _beeSpawnedCount;

        private HoneyRecycler _recycler;
        private CollectionArea _basket;

        public void StartScene()
        {
            _hivesController.OnAllHivesOpened += SpawnRecycler;
            glassFillWithHoneyMediator.OnAllGlassFilled += SpawnBasket;
            _hivesController.SpawnHives(_soundSystem, _fxSystem);
            SpawnSprayer();
            StartCoroutine(BeeSpawningRoutine());
            _soundSystem.PlaySound("Bee");
            _soundSystem.InitLevelMusic();
        }

        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _hivesController.OnAllHivesOpened += SpawnRecycler;
            glassFillWithHoneyMediator.OnAllGlassFilled += SpawnBasket;
            _hivesController.SpawnHives(_soundSystem, _fxSystem);
            SpawnSprayer();
            StartCoroutine(BeeSpawningRoutine());
            _soundSystem.PlaySound("Bee");
            _soundSystem.InitLevelMusic();
        }

        // Spawns the recycler
        private void SpawnRecycler()
        {
            _recycler = _recyclerSpawner.SpawnRecycler(_soundSystem, _inputSystem);
            glassFillWithHoneyMediator
                .Init(_recycler.GetComponentInChildren<HoneyTap>());

            _recycler.MoveToDestination();
            _recycler.OnAllHoneyRecycled += AllHoneyRecycled;
            _recycler.StartRecycling += _hivesController.MakeAllHoneyCombsNonInteractable;
            _recycler.EndRecycling += _hivesController.MakeAllHoneyCombsInteractable;
        }

        // Handles the event when all honey is recycled, triggers camera zoom, executes mediator, and shows a hint
        private void AllHoneyRecycled()
        {
            _camera.ZoomCamera().OnComplete(() =>
            {
                glassFillWithHoneyMediator.FirstExecute(_soundSystem, _fxSystem);
                ActivateHint(_recycler.HintPosition, null, 0.5f);
            });
        }

        // Coroutine for spawning bees at a specific rate
        private IEnumerator BeeSpawningRoutine()
        {
            ActivateHint(_sprayerSpawner.HintPoint, Vector3.zero);
            while (_config.BeeToSpawn > _beeSpawnedCount)
            {
                yield return new WaitForSeconds(_beeSpawnRate);

                var bee = _beeSpawner.SpawnBee();
                bee.GetComponent<LoopMoving>().StartMoving();

                _beeSpawnedCount++;
            }
        }

        // Spawns the sprayer 
        private void SpawnSprayer()
        {
            Sprayer sprayer = _sprayerSpawner.SpawnSprayer(_soundSystem, _fxSystem);
            sprayer.GetComponent<MoveStartDestination>().MoveToDestination();
            sprayer.OnAllBeesCatch += _hivesController.InitHives;
            sprayer.OnAllBeesCatch += _actorUI.HideProgressBar;
            _actorUI.InitProgressBar(sprayer);
        }

        // Spawns the basket and sets up its properties, sorting
        private void SpawnBasket()
        {
            _basket = _basketSpawner.SpawnBasket(_fxSystem, _soundSystem);
            _actorUI.InitWinInvoker(_basket);
            _basket.STORED_MAX_COUNT = _config.GlassToSpawn;
            _basket.SetSortingIndex();
            _basket.AddComponent<HoneyGlassBasketTriggerObserver>().OnTriggerEnter += _basket.StoreObj;
            _basket.GetComponent<MoveStartDestination>().MoveToDestination();
        }

        private void OnDestroy()
        {
            if(_basket != null)
                _basket.GetComponent<HoneyGlassBasketTriggerObserver>().OnTriggerEnter -= _basket.StoreObj;
        }
    }
}