using Carrot.Spawners;
using Carrot.Config;
using System.Collections.Generic;
using Unity.VisualScripting;
using System.Linq;
using UnityEngine;

namespace Carrot
{
    public class CarrotLevelMediator : BaseLevelMediator, IStartScenable
    {
        [Header("Carrot Mediator")]
        [SerializeField] private CarrotLevelConfig _config;
        [SerializeField] private CarrotBasketSpawner _carrotBasketSpawner;
        [SerializeField] private CarrotSpawner _carrotSpawner;
        [SerializeField] private SeedPackageSpawner _seedPackageSpawn;
        [SerializeField] private WaterPumpSpawner _pumpSpawner;
        [SerializeField] private SeedSpawner _seedSpawner;
        [SerializeField] private CarrotHolesContainer _holesContainer;
        [SerializeField] private MoleSpawnController _moleSpawn;

        private SeedPackage _seedPackage;
        private WaterPump _waterPump;
        private CollectionArea basket;
        private List<Carrot> _carrots = new();

        public void StartScene()
        {
            _actorUI.InitProgressBar(_moleSpawn);
            _moleSpawn.StartMoleSpawning();
            _moleSpawn.OnWholeMoleCatch += WholeMoleCatch;

            RegisterCarrotSpawner();
            _soundSystem.InitLevelMusic();
        }

        // initialize progress bar in [ActroUI], launch mole spawning, monitor whether all moles have been caught
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _actorUI.InitProgressBar(_moleSpawn);
            _moleSpawn.StartMoleSpawning();
            _moleSpawn.OnWholeMoleCatch += WholeMoleCatch;

            RegisterCarrotSpawner();
            _soundSystem.InitLevelMusic();
        }

        private void OnDestroy()
        {
            _moleSpawn.OnWholeMoleCatch -= WholeMoleCatch;
            if(basket != null)
                basket.GetComponent<CarrotTriggerObserver>().OnTriggerEnter -= basket.StoreObj;
        } 

        // all moles have been caught so start spawn SeedPackage and monitor whether first seed has putted into hole
        private void WholeMoleCatch()
        {
            _holesContainer.HolesBlink(true);
            _actorUI.HideProgressBar();
            _seedPackage = _seedPackageSpawn.SpawnPackage();
            _seedPackage.OnPutInHole += SpawnSeed;
            _seedPackage.OnPutInHole += StopHoleBlink;
            _seedPackage.OnSeedBuried += SpawnWaterPump;
            _seedSpawner.OnSeedSpawn += _seedPackage.PutInHole;
            _carrotSpawner.OnSpawn += SpawnBasket;
        }

        // disable hole red blinking animation 
        private void StopHoleBlink(Vector3 start = default)
        {
            _holesContainer.HolesBlink(false);
            _seedPackage.OnPutInHole -= StopHoleBlink;
        }

        // enable hole red blinking animation 
        private void StartBlink()
        {
            _holesContainer.HolesBlink(true);
            _waterPump.OnPosition -= StartBlink;
        }

        private void StopBurryDirtBlink()
            => _holesContainer.HolesBlink(false);
        
        // make all carrots available for dragging
        private void MakeCarrotsInteractable()
        {
            foreach (var carrot in _carrots)
                carrot.MakeInteractable();
        }

        // SeedPackage leave scene, enable holes collider, spawn WaterPump and move to scene it, enable hint on WaterPump 
        private void SpawnWaterPump()
        {
            if (!IsHolesFull())
                return;

            _seedPackage.EndLifeCycle();
            _holesContainer.EnableHolesCollider();
            _waterPump = _pumpSpawner.SpawnWaterPump();
            _waterPump.MoveToDestination();
            _waterPump.OnPosition += StartBlink;
            ActivateHint(_waterPump.Destination, _holesContainer.HolesOnScene[0].transform.position);
        }

        // check if the hole is filled with water, if so, spawn carrot
        private void RegisterCarrotSpawner()
        {
            foreach (var hole in _holesContainer.HolesOnScene)
                hole.OnHoleFillWithWater += SpawnCarrot;
        }

        // spawn carrot and monitor whether carrot has been picked, if so, destroy this hole and disable hint
        private void SpawnCarrot(CarrotHole carrotHole, Vector3 position)
        {
            StopBurryDirtBlink();

            Carrot carrot = _carrotSpawner.SpawnCarrot(position);
            carrot.Dragging += carrotHole.DestroyHole;
            carrot.Dragging += _carrotBasketSpawner.DisableBasketArrowHint;
            carrot.Dragging += DisableHint;
        }
        
        // spawn basket, add [Carrot] in [_carrots] List, enable hint on carrot, WaterPump leave scene,
        // make all carrots available for dragging
        private void SpawnBasket(Carrot carrot)
        {
            _carrots.Add(carrot);

            if (!_holesContainer.IsAllHolesFillWithWater())
                return;
            
            ActivateHint(carrot.transform.position, carrot.transform.position);
            _waterPump.EndLifeCycle();
            basket = _carrotBasketSpawner.SpawnBasket(_fxSystem, _soundSystem);
            _actorUI.InitWinInvoker(basket);
            basket.STORED_MAX_COUNT = _config.CatchMoleToWin;
            basket.SetSortingIndex();
            basket.AddComponent<CarrotTriggerObserver>().OnTriggerEnter += basket.StoreObj;
            MakeCarrotsInteractable();
        }

        // check if all holes have been filled with water, if so, return "true"
        private bool IsHolesFull()
            => _holesContainer.HolesOnScene.All(hole => hole.IsFull);

        private void SpawnSeed(Vector3 start)
            => _seedSpawner.SpawnSeed(start);
    }
}