using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Tomato.Spawners;
using UnityEngine;
using UsefulComponents;
using Random = UnityEngine.Random;
using WaterPumpSpawner = Tomato.Spawners.WaterPumpSpawner;


namespace Tomato
{
    public class TomatoMediator : BaseLevelMediator, IStartScenable
    {
        private const string SUNSHINE = "Sunshine";
        private const int TOMATO_COUNT = 8;

        [Header("Tomato Mediator")]
        [SerializeField] private Sun _sun;
        [SerializeField] private GameObject _tomato;
        [SerializeField] private List<TomatoSeed> _spawnedSeeds = new();
        [SerializeField] private TomatoLevelConfig _tomatoConfig;
        [SerializeField] private TomatoHolesContainer _holes;
        [SerializeField] private TomatoSeedSpawner _tomatoSeedSpawner;
        [SerializeField] private TomatoBasketSpawner _tomatoBasketSpawner;
        [SerializeField] private WormsBasketSpawner _wormsBasketSpawner;
        [SerializeField] private WaterPumpSpawner _waterPumpSpawner;
        [SerializeField] private ShovelSpawner _shovelSpawner;
        [SerializeField] private WormSpawner _wormsSpawner;
        [SerializeField] private WaterPump _waterPump;
        [SerializeField] private float _sunDuration;
        [SerializeField] private GameObject _tomatoPool;

        private TomatoTriggerObserver _tomatoObserver;
        private CollectionArea _tomatoBasket;
        private TomatoShovel _shovel;
        private int _tomatoSpawned;
        private int _tomatoGrowed;
        private int _spawnedWorms;

        public void StartScene()
        {
            _soundSystem.InitLevelMusic();
            _shovel = _shovelSpawner.SpawnShovel();
            _tomatoObserver = GetComponent<TomatoTriggerObserver>();

            RegisterHoles(_shovel);

            _holes.OnAllHolesReady += _shovel.EndLifeCycle;
            _holes.OnAllHolesReady += SpawnSeed;
            _holes.OnAllHolesReady += () => ActivateHint(_tomatoSeedSpawner.GetDestinationPoint(), GetPositionFromHoles());

            ActivateHint(_shovel.GetDestination(), GetPositionFromHoles());
        }
        /// <summary>
        /// Викликає лопату, присвоює події ями "OnAllHolesReady" ф-ції "SpawnSeed", "EndLifeCycle", "SetHint"
        /// та викликає підказку
        /// </summary>
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _soundSystem.InitLevelMusic();
            _shovel = _shovelSpawner.SpawnShovel();
            _tomatoObserver = GetComponent<TomatoTriggerObserver>();

            RegisterHoles(_shovel);

            _holes.OnAllHolesReady += _shovel.EndLifeCycle;
            _holes.OnAllHolesReady += SpawnSeed;
            _holes.OnAllHolesReady += () => ActivateHint(_tomatoSeedSpawner.GetDestinationPoint(), GetPositionFromHoles());

            ActivateHint(_shovel.GetDestination(), GetPositionFromHoles());
        }

        /// <summary>
        /// Видаляє з події "OnTriggerEnter" ф-цію "StoreObj"
        /// </summary>
        private void OnDestroy()
        {
            if(_tomatoBasket != null)
                _tomatoObserver.OnTriggerEnter -= _tomatoBasket.StoreObj;
        }

        /// <summary>
        /// Повертає позицію з ям [_holes]
        /// </summary>
        private Vector3 GetPositionFromHoles()
            => _holes.HolesOnScene[Random.Range(0, _holes.HolesOnScene.Count)].StorePosition;

        /// <summary>
        /// Вводимо лопату [shovel] - додає події "OnLifeCycleEnded" ф-цію "MakeInteractable"
        /// </summary>
        /// <param name="shovel">лопата</param>
        private void RegisterHoles(TomatoShovel shovel)
        {
            foreach (var hole in _holes.HolesOnScene)
            {
                shovel.OnLifeCycleEnded += hole.MakeInteractable;
            }
        }

        /// <summary>
        /// Викликає саженець томату [tomato seed]
        /// </summary>
        private void SpawnSeed()
        {
            _holes.MakeHolesInterable();
            TomatoSeed seed = new TomatoSeed();
            if (_tomatoSeedSpawner.HasFreeSeeds())
            {
                seed = _tomatoSeedSpawner.SpawnSeed();
                seed.MoveToDestination();
                seed.SetTomatoPool(_tomatoPool);
                _spawnedSeeds.Add(seed);
            }

            if (_tomatoSeedSpawner.HasFreeSeeds())
            {
                seed.OnStored += SpawnSeed;
                return;
            }

            seed.OnStored += SpawnWaterPump;
        }

        /// <summary>
        /// Викликає лійку [_waterPump] та підказку
        /// </summary>
        private void SpawnWaterPump()
        {
            _waterPump = _waterPumpSpawner.SpawnWaterPump();
            _waterPump.MoveToDestination();
            ActivateHint(_waterPumpSpawner.GetDestinationPoint(), GetPositionFromHoles());
        }

        /// <summary>
        /// Вводимо саженець томату [tomatoSeed] й значення прогресу [value] - 
        /// створюємо помідор на саженці томату [tomatoSeed]
        /// </summary>
        /// <param name="tomatoSeed"></param>
        /// <param name="value"></param>
        public void CreateTomato(TomatoSeed tomatoSeed, float value = 0)
        {
            if (!tomatoSeed.fullWet)
            {
                tomatoSeed.WetAnimation();
                tomatoSeed.Appear();

                tomatoSeed.OnTomatosGrowed += () => AddTomatoGrowed();
                tomatoSeed.SetAction(StartSunProcess);
                tomatoSeed.OnTomatosFullGrowed += SpawnBasket;
            }
            if (_tomatoSpawned < TOMATO_COUNT)
            {
                CreateTomato(tomatoSeed._tomatoSpawnPoints, tomatoSeed, value);
            }
        }

        /// <summary>
        /// Вводимо точки появи томатів [_tomatoSpawnPoints], саженець томатів [_currentSeed] та значення появи [valueToSpawn]
        /// - якщо на саженці томатів [_currentSeed] не з'явились всі томати, то створює томат 
        /// </summary>        
        private void CreateTomato(List<GameObject> _tomatoSpawnPoints, TomatoSeed _currentSeed, float valueToSpawn)
        {
            if (!_currentSeed.isUsing)
            {
                _currentSeed.SetValueToSpawn(valueToSpawn);

                if (_currentSeed.IsTomatoCanSpawn())
                {
                    var tomato = Instantiate(_tomato, transform.position, Quaternion.identity);

                    _tomatoSpawned++;
                    _currentSeed.AddTomatoes(tomato);
                }

                if (CheckIsTomatoesIsEnough())
                {
                    _waterPump.EndLifeCycle();
                    SpawnBasketAndWorms();
                }
            }
        }

        /// <summary>
        /// Додає к-сть створених томатів [_tomatoGrowed]
        /// </summary>
        private void AddTomatoGrowed()
            => _tomatoGrowed++;

        /// <summary>
        /// Викликає банку для черв'яків та починає появу черв'яків 
        /// </summary>
        public void SpawnBasketAndWorms()
        {
            WormsBasket wormsBasket = _wormsBasketSpawner.SpawnBasket();
            wormsBasket.GetComponent<MoveStartDestination>().MoveToDestination();
            AddActionToWarmBasket(wormsBasket);

            foreach (TomatoSeed currentSeed in _spawnedSeeds)
            {
                currentSeed.DisableInteractorCollider();
            }

            StartCoroutine(SpawnWorm());

            ActivateHint(GetPositionFromHoles(), _wormsBasketSpawner.GetDestinationPoint());
            _tomatoSeedSpawner.SetReservDestinationPointToSpawnedSeed();
        }

        /// <summary>
        /// Викликає кошик для томатів, присвоює к-сть томатів, що треба скласти [STORED_MAX_COUNT] та
        /// для події "OnTriggerEnter" додає ф-цію "StoreObj"
        /// </summary>
        private void SpawnBasket()
        {
            _tomatoBasket = _tomatoBasketSpawner.SpawnBasket(_fxSystem, _soundSystem);
            _tomatoBasket.STORED_MAX_COUNT = _tomatoConfig.MaxStoredTomato;
            print("Set off max store count");
            _tomatoBasket.SetSortingIndex();
            _tomatoBasket.AddComponent<TomatoTriggerObserver>().OnTriggerEnter += _tomatoBasket.StoreObj;
            _actorUI.InitWinInvoker(_tomatoBasket);
        }

        /// <summary>
        /// Вводимо банку для черв'яків [wormBasket]- додає ф-ції для події "OnCatchAllWorms"
        /// </summary>
        /// <param name="wormBasket">банка для черв'яків</param>
        private void AddActionToWarmBasket(WormsBasket wormBasket)
        {
            _actorUI.InitProgressBar(wormBasket);

            foreach (var seedTemp in _spawnedSeeds)
            {
                wormBasket.OnCatchAllWorms += () => seedTemp.canGrow = true;
                wormBasket.OnCatchAllWorms += seedTemp.GrowTomatos;
                wormBasket.OnCatchAllWorms += _wormsSpawner.StopMonitoring;
                wormBasket.OnCatchAllWorms += _actorUI.HideProgressBar;
            }
        }

        /// <summary>
        /// Викликає черв'яка кожні 2 секунди
        /// </summary>        
        private IEnumerator SpawnWorm()
        {
            Worm.UncatchedWormCount = _tomatoConfig.WormsToSpawn;
            while (_tomatoConfig.WormsToSpawn > _spawnedWorms)
            {
                yield return new WaitForSeconds(2);
                _spawnedWorms++;
                Worm currentWorm = _wormsSpawner.SpawnWorm();
                currentWorm.Appear();
            }
        }

        /// <summary>
        /// Перевіряє чи достатньо з'явилося томатів
        /// </summary>
        private bool CheckIsTomatoesIsEnough()
           => _tomatoSpawned >= TOMATO_COUNT;

        /// <summary>
        /// Починає показувати сонячні промені
        /// </summary>
        private void StartSunProcess()
        {
            _soundSystem.PlaySound(SUNSHINE);
            _sun.Sunshine(_sunDuration, () => _soundSystem.StopSound(SUNSHINE));
        }
    }
}
