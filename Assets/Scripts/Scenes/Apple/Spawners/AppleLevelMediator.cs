using System;
using UnityEngine;
using Unity.VisualScripting;

namespace Apple.Spawners
{
    public class AppleLevelMediator : BaseLevelMediator, IStartScenable
    {
        private const string Success = "Success";
        private const string Sunshine = "Sunshine";

        [Header("Apple Mediator")]
        [SerializeField] private Sun _sun;
        [SerializeField] private int _verminsCount;
        [SerializeField] private float ripeDuration = 5f;
        [SerializeField] private AppleHoleContainer _holes;
        [SerializeField] private AppleFertilizerContainer _fertilizerContainer;
        [SerializeField] private ShovelSpawner _shovelSpawner;
        [SerializeField] private AppleSeedlingSpawner _appleSeedlingSpawner;
        [SerializeField] private AppleWaterPumpSpawner _waterPumpSpawner;
        [SerializeField] private AppleFertilizerPackageSpawner _fertilizerPackageSpawner;
        [SerializeField] private AppleSpawner _appleSpawner;
        [SerializeField] private VerminsSpawner _verminsSpawner;
        [SerializeField] private AppleBasketSpawner _basketSpawner;
        [SerializeField] private AppleLevelConfig _config;

        private AppleWaterPump Pump { get; set; }
        private AppleFertilizerPackage Package { get; set; }
        private AppleShovel Shovel { get; set; }
        private CollectionArea _basket;
        private AppleShovel _shovel;

        public event Action OnAllVerminsKilled;
        public event Action OnEndRipe;

        public void StartScene()
        {
            SpawnApples();
            _shovelSpawner.OnShovelSpawned += ShovelSpawned;
            _holes.OnAllHolesReady += SpawnSeedling;

            _fertilizerPackageSpawner.OnPackageSpawned += _fertilizerContainer.Init;
            _fertilizerContainer.OnAllPlacesFertilized += EndFertilizingProcess;
            _fertilizerContainer.OnAllPlacesFertilized += SpawnWaterPump;
            _holes.OnAllApplesGrew += StartVerminsProcess;
            OnAllVerminsKilled += EndVerminsProcess;
            OnAllVerminsKilled += StartSunProcess;
            OnEndRipe += SpawnBasket;

            _shovel = _shovelSpawner.SpawnShovel();
            _soundSystem.InitLevelMusic();
        }

        //Subscribes to events
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();

            SpawnApples();
            _shovelSpawner.OnShovelSpawned += ShovelSpawned;
            _holes.OnAllHolesReady += SpawnSeedling;

            _fertilizerPackageSpawner.OnPackageSpawned += _fertilizerContainer.Init;
            _fertilizerContainer.OnAllPlacesFertilized += EndFertilizingProcess;
            _fertilizerContainer.OnAllPlacesFertilized += SpawnWaterPump;
            _holes.OnAllApplesGrew += StartVerminsProcess;
            OnAllVerminsKilled += EndVerminsProcess;
            OnAllVerminsKilled += StartSunProcess;
            OnEndRipe += SpawnBasket;

            _shovel = _shovelSpawner.SpawnShovel();
            _soundSystem.InitLevelMusic();
        }

        //Spawns a basket, sets its properties, adds event listeners, and makes apples interactable
        private void SpawnBasket()
        {
            var hol = _holes.HolesOnScene[0];
            var hintPos = hol.AppleAppearPoints[0].transform.position;
            
            _basket = _basketSpawner.SpawnBasket(_fxSystem, _soundSystem,hintPos);
            _actorUI.InitWinInvoker(_basket);
            _basket.STORED_MAX_COUNT = _config.MaxStoredApples;
            _basket.SetSortingIndex();
            _basket.AddComponent<AppleFruitTriggerObserver>().OnTriggerEnter += _basket.StoreObj;
            foreach (var hole in _holes.HolesOnScene)
            {
                hole.MakeApplesInteractable();
            }
            // додати підказку

            ;
        }

        //Initiates the sun process, including triggering sunshine and making apples ripe
        private void StartSunProcess()
        {
            _soundSystem.PlaySound(Sunshine);
            _sun.Sunshine(ripeDuration, EndRipe);
            foreach (var hole in _holes.HolesOnScene)
            {
                hole.RipeApples(ripeDuration);
            }
        }

        //Invokes the end ripe event and plays a success sound
        private void EndRipe()
        {
            OnEndRipe?.Invoke();
            _soundSystem.PlaySound(Success);
        }

        //Hides the progress bar for vermins
        private void EndVerminsProcess()
        {
            _actorUI.HideProgressBar();
        }

        //Disappears the holes and ends the watering process
        private void EndWateringProcess()
        {
            _fertilizerContainer.DisappearHoles();
            Pump.EndLifeCycle();
        }

        //Initializes and spawns vermins, and initializes the progress bar
        private void StartVerminsProcess()
        {
            _verminsSpawner.InitAndSpawn(_verminsCount, OnAllVerminsKilled);
            _actorUI.InitProgressBar(_verminsSpawner);
        }

        //Spawns apples in each hole
        private void SpawnApples()
        {            
            foreach (var hole in _holes.HolesOnScene)
            {
                foreach (var spawnPoint in hole.AppleAppearPoints)
                {
                    var apple = _appleSpawner.SpawnApple();
                    apple.transform.position = spawnPoint.transform.position;
                    hole.AddApple(apple);
                }
            }        
        }

        //Ends the fertilizing process
        private void EndFertilizingProcess()
        {
            Package.EndLifeCycle();
        }

        //Spawns a seedling, handles seedling events, and starts watering process
        private void SpawnSeedling()
        {
            StartSeedlingHint();
            SeedlingsApple seedling = _appleSeedlingSpawner.SpawnSeed();

            seedling.MoveToDestination();

            if (_appleSeedlingSpawner.HasFreeSeeds())
            {
                seedling.OnStored += SpawnSeedling;
                return;
            }

            seedling.OnStored += SpawnFertilizerPackage;
            seedling.OnTreeGrew += EndWateringProcess;
            seedling.OnHalfWatering += _fertilizerContainer.HalfVisibilityHoles;
        }

        //Starts the hint for seedling in each hole
        private void StartSeedlingHint()
        {
            foreach (var hole in _holes.HolesOnScene)
            {
                hole.StartSeedlingHint();
            }
        }

        //Spawns a fertilizer package
        private void SpawnFertilizerPackage()
        {
            Package = _fertilizerPackageSpawner.SpawnFertilizerPackage(_holes.HolesOnScene[0]);
        }

        //Spawns a water pump, enables holes, and starts water hint
        private void SpawnWaterPump()
        {
            Pump = _waterPumpSpawner.SpawnWaterPump();
            _holes.EnableHoles();
            _holes.StartWaterHint();
            Pump.MoveToDestination();
        }

        //Handles the shovel spawned event, activates hint, and ends shovel lifecycle
        private void ShovelSpawned(AppleShovel shovel)
        {
            Shovel = shovel;
            Shovel.OnShovelPlaced += _holes.HolesOnScene[0].ActivateHint;
            _holes.OnAllHolesReady += Shovel.EndLifeCycle;
        }

        //Unsubscribes from various events
        private void OnDestroy()
        {
            _shovelSpawner.OnShovelSpawned -= ShovelSpawned;
            _holes.OnAllHolesReady -= SpawnSeedling;
            _fertilizerPackageSpawner.OnPackageSpawned -= _fertilizerContainer.Init;
            _fertilizerContainer.OnAllPlacesFertilized -= EndFertilizingProcess;
            _fertilizerContainer.OnAllPlacesFertilized -= SpawnWaterPump;
            if(_basket != null)
                _basket.AddComponent<AppleFruitTriggerObserver>().OnTriggerEnter -= _basket.StoreObj;
            OnAllVerminsKilled -= EndVerminsProcess;
            OnAllVerminsKilled -= StartSunProcess;
            OnEndRipe -= SpawnBasket;
        }
    }
}

