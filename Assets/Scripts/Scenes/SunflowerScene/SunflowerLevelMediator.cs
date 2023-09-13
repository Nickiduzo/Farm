using System;
using System.Collections;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;
using Unity.VisualScripting;

namespace SunflowerScene
{
    public class SunflowerLevelMediator : BaseLevelMediator, IStartScenable
    {
        private const string WaterSound = "Water";
        [Header("Spawners")] [SerializeField] private CarSpawner _carSpawner;
        [SerializeField] private SunflowerSpawner _sunflowerSpawner;
        [SerializeField] private WaterPumpSpawner _waterPumpSpawner;
        [SerializeField] private PestSpawner _pestSpawner;
        [SerializeField] private SeedSpawner _seedSpawner;
        [SerializeField] private SunflowerBasketSpawner _basketSpawner;

        [Header("Pointers")] [SerializeField] private Pointer _arrowPointer;
        [SerializeField] private Transform hintCrowT;
        private bool hintCrowB;

        [Header("Scene")] [SerializeField] private SunflowerConfig _config;
        [SerializeField] private ConsistentPlantingInHole _consistentPlantingInHole;
        [SerializeField] private Sun _sun;
        [SerializeField] private Camera _camera;
        [SerializeField] private int _needKillPests;
        [SerializeField] private Transform _basketDestination;
        [SerializeField] private Transform _basketEndPosition;

        private CounterInvoker<SunflowerHole> _holePlantedInvoker;
        private CounterInvoker<PlantGrowing> _plantGrewInvoker;
        private CounterInvoker<Pest> _pestDiedInvoker;
        private CounterInvoker<SunflowerHead> _sunflowerHeadDroppedInvoker;

        private Car _car;
        private WaterPump _waterPump;
        private CollectionArea basket;
        private Basket _basket;
        private KillingPestProgress _progress;
        private SuccessEffectsHandler _successEffectsHandler;


        private SunflowerHead[] _sunflowerHeads;

        public void StartScene()
        {
            _basketDestination.position =
                _camera.ScreenToWorldPoint(new Vector3(0.13f * Screen.width, 0.23f * Screen.height, 1));
            _successEffectsHandler = new SuccessEffectsHandler(_soundSystem, _fxSystem);
            _holePlantedInvoker = new CounterInvoker<SunflowerHole>(_consistentPlantingInHole.Holes);
            _arrowPointer.Construct(_consistentPlantingInHole);
            _seedSpawner.Constructor(_inputSystem);
            _pestSpawner.Construct(_needKillPests);
            _car = _carSpawner.SpawnCar();
            _sunflowerSpawner.Construct(_inputSystem, _car.SunflowerSpawnPoint);

            _sunflowerSpawner.SpawnAllSunflowers();
            RegisterPlant();

            _soundSystem.InitLevelMusic();
            _holePlantedInvoker.AllCountedUp += RegisterWatering;
        }
        // It subscribes to events and init spawners
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _basketDestination.position =
                _camera.ScreenToWorldPoint(new Vector3(0.13f * Screen.width, 0.23f * Screen.height, 1));
            _successEffectsHandler = new SuccessEffectsHandler(_soundSystem, _fxSystem);
            _holePlantedInvoker = new CounterInvoker<SunflowerHole>(_consistentPlantingInHole.Holes);
            _arrowPointer.Construct(_consistentPlantingInHole);
            _seedSpawner.Constructor(_inputSystem);
            _pestSpawner.Construct(_needKillPests);
            _car = _carSpawner.SpawnCar();
            _sunflowerSpawner.Construct(_inputSystem, _car.SunflowerSpawnPoint);

            _sunflowerSpawner.SpawnAllSunflowers();
            RegisterPlant();

            _soundSystem.InitLevelMusic();
            _holePlantedInvoker.AllCountedUp += RegisterWatering;
        }

        private void RegisterPlant()
        {
            _car.Mover.EndMove += _arrowPointer.PointOnPosition;
            _car.Mover.StartMove += _arrowPointer.Hide;

            _holePlantedInvoker.CountedUp += _consistentPlantingInHole.Next;
            _holePlantedInvoker.CountedUp += _car.Mover.MoveToTarget;
            _holePlantedInvoker.AllCountedUp += _arrowPointer.Hide;

            _car.Mover.EndMove += _consistentPlantingInHole.UnLockCurrentHole;
            _car.Mover.EndMove += ShowPlantPointer;
            _car.Mover.StartMove += LockAllSunflowers;
            _car.Mover.EndMove += UnlockAnyNotPlantedSunflower;

            _holePlantedInvoker.CountedUp += _successEffectsHandler.PlaySuccessSound;
            _holePlantedInvoker.CountedUpIn += _successEffectsHandler.PlaySuccessEffect;
            _holePlantedInvoker.AllCountedUp += StopPlant;
            _car.Mover.Construct(_consistentPlantingInHole);
            _car.Mover.MoveToTarget();
        }

        private void UnlockAnyNotPlantedSunflower()
        {
            foreach (var sunflowerSprout in _sunflowerSpawner.GetObjects())
            {
                if (sunflowerSprout.Planted == false)
                {
                    sunflowerSprout.MakeInteractable();
                    sunflowerSprout.UpdateDestinationOnDragEnd();
                    sunflowerSprout.SetRigidbodySimulated(true);
                    break;
                }
            }
        }

        private void ShowPlantPointer()
        {
            var start = _sunflowerSpawner.GetObjects()[0].transform.position;
            var end = _consistentPlantingInHole.CurrentHole.transform.position;
            ActivateHint(start, end);
            _car.Mover.StartMove += HideHintSystem;
        }

        private void HideHintSystem()
        {
            DisableHint();
            _car.Mover.StartMove -= HideHintSystem;
            _car.Mover.EndMove -= ShowPlantPointer;
        }

        private void LockAllSunflowers()
        {
            foreach (var sunflowerSprout in _sunflowerSpawner.GetObjects())
            {
                sunflowerSprout.MakeNonInteractable();
                sunflowerSprout.SetRigidbodySimulated(false);
            }
        }

        private void StopPlant()
        {
            _car.Mover.EndMove -= _arrowPointer.PointOnPosition;
            _car.Mover.StartMove -= _arrowPointer.Hide;

            _holePlantedInvoker.CountedUp -= _consistentPlantingInHole.Next;
            _holePlantedInvoker.CountedUp -= _car.Mover.MoveToTarget;
            _holePlantedInvoker.AllCountedUp -= _arrowPointer.Hide;

            _car.Mover.EndMove -= _consistentPlantingInHole.UnLockCurrentHole;
            _car.Mover.EndMove -= ShowPlantPointer;
            _car.Mover.StartMove -= LockAllSunflowers;
            _car.Mover.EndMove -= UnlockAnyNotPlantedSunflower;

            _holePlantedInvoker.CountedUp -= _successEffectsHandler.PlaySuccessSound;
            _holePlantedInvoker.CountedUpIn -= _successEffectsHandler.PlaySuccessEffect;
            _holePlantedInvoker.AllCountedUp -= StopPlant;
            _holePlantedInvoker.AllCountedUp -= RegisterWatering;
            _car.Mover.MoveToEnd();
        }

        private void RegisterWatering()
        {
            _plantGrewInvoker = new CounterInvoker<PlantGrowing>(GetPlantGrowing());
            _waterPump = _waterPumpSpawner.SpawnWaterPump(_soundSystem);

            _plantGrewInvoker.CountedUp += _successEffectsHandler.PlaySuccessSound;
            _plantGrewInvoker.CountedUpIn += _successEffectsHandler.PlaySuccessEffect;

            _waterPump.StartedFlow += DisableSunflowerFlashHint;
            _waterPump.StartedFlow += DisableHint;

            _plantGrewInvoker.AllCountedUp += StopWatering;
            _plantGrewInvoker.AllCountedUp += RegisterPestProtection;

            var sunflowers = _sunflowerSpawner.GetObjects();
            ActivateHint(_waterPump.WaterPumpHintPoint, sunflowers[0].transform.position);

            EnableSunflowerRigidbody();
            SetSunflowerFlashing(true);
        }

        private PlantGrowing[] GetPlantGrowing()
        {
            var sunflowers = _sunflowerSpawner.GetObjects();
            var plants = new PlantGrowing[sunflowers.Length];

            for (var index = 0; index < plants.Length; index++)
            {
                plants[index] = sunflowers[index].PlantGrowing;
                StartCoroutine(IncludeAnimationPlant(sunflowers[index].GetComponent<Animation>(), index));
            }

            return plants;
        }

        IEnumerator IncludeAnimationPlant(Animation anim, int num)
        {
            yield return new WaitForSeconds(1.3f);
            if (num > 2)
            {
                anim.clip = anim.GetClip("SunflowerSproutIdle2");
                anim.Play();
            }

            anim.enabled = true;
        }

        private void EnableSunflowerRigidbody()
        {
            foreach (var sunflower in _sunflowerSpawner.GetObjects())
            {
                sunflower.SetRigidbodySimulated(true);
            }
        }

        private void DisableSunflowerFlashHint()
        {
            Debug.Log("DisableSunflowerFlashHint");
            SetSunflowerFlashing(false);
            _waterPump.StartedFlow -= DisableSunflowerFlashHint;
        }

        private void SetSunflowerFlashing(bool isFlashing)
        {
            foreach (var sunflower in _sunflowerSpawner.GetObjects())
            {
                sunflower.ActivateFlash(isFlashing);
            }
        }

        private void StopWatering()
        {
            //_waterPump.StopFlow();
            _plantGrewInvoker.CountedUp -= _successEffectsHandler.PlaySuccessSound;
            _plantGrewInvoker.CountedUpIn -= _successEffectsHandler.PlaySuccessEffect;


            _plantGrewInvoker.AllCountedUp -= StopWatering;
            _plantGrewInvoker.AllCountedUp -= RegisterPestProtection;

            _waterPumpSpawner.HideWaterPump(_waterPump);
            // просто зупинив з SoundSystemUser
            SoundSystemUser.Instance.StopSound(WaterSound);
        }

        private void RegisterPestProtection()
        {
            _progress = new KillingPestProgress(_needKillPests);
            _actorUI.InitProgressBar(_progress);
            _pestDiedInvoker = new CounterInvoker<Pest>(_needKillPests);
            _pestSpawner.PestSpawned += _pestDiedInvoker.Add;
            _pestDiedInvoker.CountedUp += _progress.AddKilledPest;

            _pestDiedInvoker.AllCountedUp += _pestSpawner.StopSpawn;
            _pestDiedInvoker.AllCountedUp += HideAllPest;
            _pestDiedInvoker.AllCountedUp += ShowSun;
            _pestDiedInvoker.AllCountedUp += _actorUI.HideProgressBar;

            _pestDiedInvoker.CountedUp += _successEffectsHandler.PlaySuccessSound;
            _pestDiedInvoker.CountedUpIn += _successEffectsHandler.PlaySuccessEffect;

            _pestDiedInvoker.CountedUp += HintHide;
            Invoke(nameof(ShowHide), 4f);
            _pestSpawner.StartSpawn();
            // HidePointerHint();
        }

        private void ShowHide()
        {
            if (hintCrowB)
                return;
            var position = hintCrowT.position;
            ActivateHint(position, position);
        }

        private void HintHide()
        {
            hintCrowB = true;
            DisableHint();
            _pestDiedInvoker.CountedUp -= HintHide;
        }

        private void HideAllPest()
        {
            foreach (var pest in _pestSpawner.GetObjects())
            {
                pest.PathMover.StopMove();
            }
        }

        private void ShowSun()
            => _sun.Sunshine(StopPestProtection);

        private void StopPestProtection()
        {
            _pestSpawner.PestSpawned -= _pestDiedInvoker.Add;
            _pestDiedInvoker.CountedUp -= _progress.AddKilledPest;

            _pestDiedInvoker.AllCountedUp -= _pestSpawner.StopSpawn;
            _pestDiedInvoker.AllCountedUp -= HideAllPest;
            _pestDiedInvoker.AllCountedUp -= ShowSun;
            _pestDiedInvoker.AllCountedUp -= _actorUI.HideProgressBar;

            _pestDiedInvoker.CountedUp -= _successEffectsHandler.PlaySuccessSound;
            _pestDiedInvoker.CountedUpIn -= _successEffectsHandler.PlaySuccessEffect;
            RegisterDropSunflowerHeads();
        }

        private void RegisterDropSunflowerHeads()
        {
            _sunflowerHeads = GetHeads();
            ActivateHead(_sunflowerHeads);
            _sunflowerHeadDroppedInvoker = new CounterInvoker<SunflowerHead>(_sunflowerHeads);
            ActivateHint(_sunflowerHeads[0].transform.position);

            _sunflowerHeadDroppedInvoker.CountedUpIn += _seedSpawner.Spawn;
            _sunflowerHeadDroppedInvoker.CountedUp += DisableHint;
            _sunflowerHeadDroppedInvoker.CountedUp += _successEffectsHandler.PlaySuccessSound;
            _sunflowerHeadDroppedInvoker.CountedUpIn += _successEffectsHandler.PlaySuccessEffect;
            _sunflowerHeadDroppedInvoker.AllCountedUp += StopDropSunflowerHeads;
            _sunflowerHeadDroppedInvoker.AllCountedUp += RegisterSeedCollection;
        }

        private void ActivateHead(SunflowerHead[] sunflowers)
        {
            foreach (var sunflower in sunflowers)
            {
                sunflower.Activate();
            }
        }

        private SunflowerHead[] GetHeads()
        {
            SunflowerHead[] heads = new SunflowerHead[_sunflowerSpawner.GetObjects().Length];
            var objects = _sunflowerSpawner.GetObjects();
            for (var index = 0; index < objects.Length; index++)
            {
                heads[index] = objects[index].SunflowerHead;
                heads[index].Construct(_soundSystem);
            }

            return heads;
        }

        private void StopDropSunflowerHeads()
        {
            _sunflowerHeadDroppedInvoker.CountedUpIn -= _seedSpawner.Spawn;
            _sunflowerHeadDroppedInvoker.CountedUp -= DisableHint;
            _sunflowerHeadDroppedInvoker.CountedUp -= _successEffectsHandler.PlaySuccessSound;
            _sunflowerHeadDroppedInvoker.CountedUpIn -= _successEffectsHandler.PlaySuccessEffect;
            _sunflowerHeadDroppedInvoker.AllCountedUp -= StopDropSunflowerHeads;
            _sunflowerHeadDroppedInvoker.AllCountedUp -= RegisterSeedCollection;
        }

        private void RegisterSeedCollection()
        {
            basket = _basketSpawner.SpawnBasket(_fxSystem, _soundSystem);
            _actorUI.InitWinInvoker(basket);
            basket.STORED_MAX_COUNT = _config.MaxStoredSeeds;
            basket.SetSortingIndex();
            basket.AddComponent<BasketTriggerObserver>().OnTriggerEnter += basket.StoreObj;
            //ShowBasket();
            //ShowSeedCollectionPointer();
            //_hintSystem.HidePointerHint();
            //_arrowPointer.Construct(basket);
            /*_actorUI.InitWinInvoker(basket);

            _basket.Construct(_sunflowerHeads.Length);

            _basket.SeedCollected += _successEffectsHandler.PlaySuccessSound;
            
            _basket.SeedCollected += _arrowPointer.Hide;
            _basket.SeedCollectedIn += _successEffectsHandler.PlaySuccessEffect;
            _basket.AllNeedSeedCollected += HideBasket;
            _basket.AllNeedSeedCollected += StopSeedCollection;*/
        }

        private void ShowBasket()
        {
            //_basket.Mover.MoveTo(_basketDestination.position).OnComplete(ShowSeedCollectionPointer);
        }

        private void ShowSeedCollectionPointer()
        {
            Vector3 start = _seedSpawner.GetEnySeedPosition();
            Vector3 end = basket.transform.position;
            ActivateHint(start, end);
            _arrowPointer.PointOnPosition();
        }

        private void HideBasket()
        {
            //_basket.Mover.ExitScene(_basketEndPosition.position);
        }

        private void StopSeedCollection()
        {
            /*_basket.SeedCollected -= _successEffectsHandler.PlaySuccessSound;
            _basket.SeedCollected -= _hintSystem.HidePointerHint;
            _basket.SeedCollected -= _arrowPointer.Hide;
            _basket.SeedCollectedIn -= _successEffectsHandler.PlaySuccessEffect;
            _basket.AllNeedSeedCollected -= HideBasket;
            _basket.AllNeedSeedCollected -= StopSeedCollection;*/
        }

        private void OnDisable()
        {
            if (_sunflowerHeadDroppedInvoker != null)
            {
                basket.AddComponent<BasketTriggerObserver>().OnTriggerEnter -= basket.StoreObj;
                _sunflowerHeadDroppedInvoker.CountedUpIn -= _seedSpawner.Spawn;
                _sunflowerHeadDroppedInvoker.CountedUp -= DisableHint;
                _sunflowerHeadDroppedInvoker.CountedUp -= _successEffectsHandler.PlaySuccessSound;
                _sunflowerHeadDroppedInvoker.CountedUpIn -= _successEffectsHandler.PlaySuccessEffect;
                _sunflowerHeadDroppedInvoker.AllCountedUp -= StopDropSunflowerHeads;
                _sunflowerHeadDroppedInvoker.AllCountedUp -= RegisterSeedCollection;
            }
        }
    }
}