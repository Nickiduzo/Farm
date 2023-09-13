using DG.Tweening;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Fishing.Spawners
{
    public class FishingLevelMediator : BaseLevelMediator, IStartScenable
    {
        [Header("Fishing Mediator")]
        [SerializeField] private FishActorUI _fishActorUI;
        [SerializeField] private BoatSpawner _boatSpawner;
        [SerializeField] private FishSpawner _fishSpawner;
        [SerializeField] private FishLevelConfig _fishConfig;
        [SerializeField] private FishingNetSpawner _netSpawner;
        [SerializeField] private FishBasketSpawner _basketSpawner;
        [SerializeField] private NoFishesMover _crab;
        [SerializeField] private NoFishesMover _octopus;
        [SerializeField] private Image fadeImage;

        [Space(10)]
        [Header("Extras")]
        [SerializeField] private Transform _startHintPos;

        private FishingNet _net;
        private Hook _hook;
        private Animator _boatAnimator;
        private CollectionArea _basket;
        
        public void StartScene()
        {
            _soundSystem.InitLevelMusic();
            InitHUDSpawn();
            _netSpawner.OnSpawn += NetSpawned;
            _boatSpawner.OnSpawn += BoatSpawned;
            _boatSpawner.SpawnBoat();
            StartHint();
        }
        /// <summary>
        /// Викликає ф-ції "InitHUDSpawn", "StartHint" та 
        /// присвоює події "OnSpawn" для сітки та човна ф-ції "NetSpawned", "BoatSpawned"
        /// </summary>
        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            _soundSystem.InitLevelMusic();
            InitHUDSpawn();
            _netSpawner.OnSpawn += NetSpawned;
            _boatSpawner.OnSpawn += BoatSpawned;
            _boatSpawner.SpawnBoat();
            StartHint();
        }

        /// <summary>
        /// Викликає ф-ції "ShowProgressBar", "AppearFishCounter" та "SpawnNet"
        /// </summary>
        private void InitHUDSpawn()
        {
            var sequence = DOTween.Sequence();
            sequence.AppendCallback(_actorUI.ShowProgressBar);
            sequence.AppendInterval(0.5f);
            sequence.AppendCallback(_fishActorUI.AppearFishCounter);
            sequence.AppendCallback(_netSpawner.SpawnNet);
        }

        /// <summary>
        /// Вводимо гачок [hook] та аніматор [animator] -
        /// запам'ятовуємо параметри, викликаємо ф-цію "InitProgressBar" та додаємо ф-цію "HideHint" події "OnDoHook"
        /// </summary>        
        private void BoatSpawned(Hook hook, Animator animator)
        {
            _hook = hook;
            _boatAnimator = animator;
            _hook.OnDoHook += HideHint;
            _actorUI.InitProgressBar(hook);
        }

        /// <summary>
        /// Вводимо сітку [net]- 
        /// запам'ятовуємо сітку в полі "_net", викликаємо ф-ції "AppearFishCounter", "NetSpawned" та
        /// додаємо ф-цію "SpawnBasket" до події "OnCatchEnough"
        /// </summary>        
        private void NetSpawned(FishingNet net)
        {
            _net = net;
            _net.OnCatchEnough += SpawnBasket;
            _net.OnCatchEnough += MoveNoFishesToEdges;
            _fishActorUI.AppearFishCounter();
            _hook.NetSpawned(net);
        }

        private void MoveNoFishesToEdges()
        {
            _crab.MoveToEdges();
            _octopus.MoveToEdges();
        }
        
        /// <summary>
        /// Виконуємо ф-цію "SpawnFishBasket", присвоюємо необхідну для збирання к-сть необхідної риби [STORED_MAX_COUNT],
        /// викликаємо ф-цію "SetSortingIndex" та додаємо ф-цію "StoreObj" до події "OnTriggerEnter"
        /// </summary>
        private void SpawnBasket()
        {
            _basket = _basketSpawner.SpawnFishBasket();
            _actorUI.InitWinInvoker(_basket);
            _basket.STORED_MAX_COUNT = _fishConfig.FishSpawnCount;
            _basket.SetSortingIndex();
            _basket.AddComponent<FishTriggerObserver>().OnTriggerEnter += _basket.StoreObj;
            BasketSpawned(_basket);
        }

        /// <summary>
        /// Вводимо кошик [basket] -
        /// Додаємо ф-ції до подій кошика та викликаємо ф-ції "DisappearFishes", "UpdateHUD", "InitWinInvoker"
        /// </summary>        
        private void BasketSpawned(CollectionArea basket)
        {
            _fishSpawner.DisappearFishes();
            _basket = basket;
            _basket.OnBasketArrived += _net.MakeCaughtFishesDraggable;
            _basket.OnBasketArrived += _boatSpawner.OnLevelCompleted.Invoke;
            _basket.OnBasketLeft += _net.Disappear;
            _basket.OnAllProductsStored += InitWinInvoker;
            _actorUI.InitWinInvoker(basket);
            UpdateHUD();
        }
        
        /// <summary>
        /// Виконує ф-цію "InitWinInvoker"
        /// </summary>
        private void InitWinInvoker()
        {
            _actorUI.InitWinInvoker(_basket);
        }

        /// <summary>
        /// Показує підказку на позиції "_startHintPos.position"
        /// </summary>
        private void StartHint()
        {
            ActivateHint(_startHintPos.position);
            _boatAnimator.SetBool("IsFishing", false);
        }

        /// <summary>
        /// Ховає підказку
        /// </summary>
        private void HideHint()
        {
            DisableHint();
            _boatAnimator.SetBool("IsFishing", true);
        }

        /// <summary>
        /// Виконує ф-цію "HideProgressBar" та змінює значення прозорості на 0.85f тривалістю 1 секунду
        /// </summary>
        private void UpdateHUD()
        {
            _actorUI.HideProgressBar();
            fadeImage.DOFade(0.85f, 1f);
        }

        /// <summary>
        /// Видаляє ф-ції з подій
        /// </summary>
        private void OnDestroy()
        {
            _netSpawner.OnSpawn -= NetSpawned;
            _boatSpawner.OnSpawn -= BoatSpawned;
            if(_basket != null)
            {
                var triggerObserver = _basket.GetComponent<FishTriggerObserver>();
                triggerObserver.OnTriggerEnter -= _basket.StoreObj;
            }
        }
    }
}