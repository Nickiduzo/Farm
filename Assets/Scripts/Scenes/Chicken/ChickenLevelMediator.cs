using System.Collections.Generic;
using System.Collections;
using ChickenScene.Entities;
using ChickenScene.Spawners;
using UnityEngine;
using DG.Tweening;

namespace ChickenScene
{
    public class ChickenLevelMediator : BaseLevelMediator, IStartScenable
    {

        [Header("Chicken Mediator")]
        [SerializeField] private GameObject _additionalTips; // arrow hint for basket 
        [SerializeField] private ChickenBasketSpawner _chickenBasketSpawner;
        [SerializeField] private List<Chicken> _chickens;
        [SerializeField] private WormsSpawner _wormsSpawner;
        [SerializeField] private EggSpawner _eggSpawner;
        
        private EatenWormsCounter _eatenWormsCounter;
        private StoredEggsCounter _storedEggsCounter;
        private ChickenBasket _basket;
        private Egg _egg;
       public void StartScene()
        {
            _eatenWormsCounter = new EatenWormsCounter(_wormsSpawner.WormsToSpawn);
            _storedEggsCounter = new StoredEggsCounter(_eggSpawner.EggsToWin);
            
            _actorUI.InitProgressBar(_eatenWormsCounter);
            _actorUI.InitWinInvoker(_storedEggsCounter);
            
            InitializedChickens();
            _wormsSpawner.SpawnWorm();
            
            _eatenWormsCounter.OnAllWormsEaten += AllWormsEaten;
            _storedEggsCounter.OnAllEggsStored += StoredAllEggs;
            _chickenBasketSpawner.OnSpawn += BasketSpawned;
            WormChicken.OnWormPickedUp += DisableWormHint;
            _soundSystem.InitLevelMusic();
        }
        // monitor whether eggs have been collected and whether all worms have been spawned, spawn first worm
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _eatenWormsCounter = new EatenWormsCounter(_wormsSpawner.WormsToSpawn);
            _storedEggsCounter = new StoredEggsCounter(_eggSpawner.EggsToWin);
            
            _actorUI.InitProgressBar(_eatenWormsCounter);
            _actorUI.InitWinInvoker(_storedEggsCounter);
            
            InitializedChickens();
            _wormsSpawner.SpawnWorm();
            
            _eatenWormsCounter.OnAllWormsEaten += AllWormsEaten;
            _storedEggsCounter.OnAllEggsStored += StoredAllEggs;
            _chickenBasketSpawner.OnSpawn += BasketSpawned;
            WormChicken.OnWormPickedUp += DisableWormHint;
            _soundSystem.InitLevelMusic();
        }

        // if chicken ate worm, create a new one and update progress bar
        private void InitializedChickens()
        {            
            foreach (var chicken in _chickens)
            {
                chicken.Consruct();
                chicken.OnWormEaten += _wormsSpawner.SpawnWorm;
                chicken.OnWormEaten += _eatenWormsCounter.UpdateProgress;
                _eatenWormsCounter.OnHalfWormsEaten += chicken.CanStillEat;
            }
        }


        // if all worms have beeb eaten, hide and reset progress bar, spawn basket
        private void AllWormsEaten()
        {
            _actorUI.HideProgressBar();
            DOVirtual.DelayedCall(1f, () =>
            {
                _actorUI.ResetProgressBar();
            });
            _chickenBasketSpawner.SpawnChickenBasket();
        }
        
        // check whether basket has arrived on scene and if so, start spawn eggs
        private void BasketSpawned(ChickenBasket basket)
        { 
            _basket = basket;
            _basket.OnArrived += BasketArrived;
            _eggSpawner.OnSpawn += EggSpawned;
            _eggSpawner.GetIndex += ChoiceOfChicken;
            StartCoroutine(TrackingBasketPosition(basket));
        }

        // enable basket hint arrow and start spawn eggs
        private void BasketArrived()
        {
            _additionalTips.SetActive(true);
            _actorUI.InitProgressBar(_storedEggsCounter);
            _eggSpawner.StartSpawnEgg();
        }

        // monitor basket position and disable arrow hint if its position is beyond set borders
        private IEnumerator TrackingBasketPosition(ChickenBasket basket)
        {
            while (true)
            {
                if(basket.transform.position.x > -15 || basket.transform.position.x < -20f)
                    DisableBasketHints();
                
                yield return new WaitForSeconds(0.1f);
            }
        }

        private void DisableBasketHints() 
            => _additionalTips.SetActive(false);

        private void DisableWormHint()
        {
            DisableHint();
            _wormsSpawner._firstWormDestroyed = true;
        }

        // select chicken for play "SpawnEggAnim" and "FeathersFx" based on [index]
        private void ChoiceOfChicken(int index)
            => _chickens[index].SpawnEggAnimAndFeathersFx(index);

        // monitor whether egg has been collected, if so, update eggs counter
        private void EggSpawned(Egg egg)
        { 
            _egg = egg;
            _egg.OnEggStored += _storedEggsCounter.UpdateProgress;
        }

        // stop spawn egg, hide progress bar and basket leave scene
        private void StoredAllEggs()
        {
            _eggSpawner.AllEggsStored = true;
            _eggSpawner.StopSpawnEgg();
            _basket.AllEggsCollected();
            _actorUI.HideProgressBar();
        }

        // unsubscribe from all events (if game was stopped in editor before basket was spawned, error "NullReferenceException"
        // will appear, this is ok, because basket has not yet been spawned, so Unity can't find reference to it)
        private void OnDestroy()
        {
            foreach (var chicken in _chickens)
            {
                chicken.Dispose();
                chicken.OnWormEaten -= _wormsSpawner.SpawnWorm;
                chicken.OnWormEaten -= _eatenWormsCounter.UpdateProgress;
                _eatenWormsCounter.OnHalfWormsEaten -= chicken.CanStillEat;
            }

            _eatenWormsCounter.OnAllWormsEaten -= AllWormsEaten;
            _storedEggsCounter.OnAllEggsStored -= StoredAllEggs;
            WormChicken.OnWormPickedUp -= DisableWormHint;
            _chickenBasketSpawner.OnSpawn -= BasketSpawned;
            _eggSpawner.GetIndex -= ChoiceOfChicken;
            _eggSpawner.OnSpawn -= EggSpawned;

            if(_basket != null)
            {
                _basket.OnArrived -= BasketArrived;
                _basket.Dispose();
            }
            
            if(_egg != null)
                _egg.OnEggStored -= _storedEggsCounter.UpdateProgress;
        }
    }
}