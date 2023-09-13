using Adveritisement;
using DG.Tweening;
using Quest;
using Scene;
using UnityEngine;
using AwesomeTools.Sound;

namespace Finish
{
    public class FinishActor : MonoBehaviour, IStartScenable
    {
        [SerializeField] private ReloadTaskButton _reloadTaskButton;
        [SerializeField] private ShopperSpawner _shopperSpawner;
        [SerializeField] private Transform _shoperDestination;
        [SerializeField] private QuestActor _questActor;
        [SerializeField] private QuestLevelConfig _config;
        [SerializeField] SoundSystem _soundSystem;

        private AdvertisementService _advertisementService;
        private Shopper _finishShopper;
        private const string previousScene = "previousScene";
        private const float START_INTERNAL_TIME = 4.6f;
        private const float LEVEL_NOT_COMPLIED_INTERNAL_TIME = 0f;
        
        private void Start()
        {
            _reloadTaskButton.OnQuestReloaded += InitFinishShopper;
            _soundSystem.InitLevelMusic();
        }

        // invoke ShopperSpawner, depending on which scene we got to "QuestScene" from
        public void StartScene()
        {
            _advertisementService = FindObjectOfType<AdvertisementService>();
            
            if (_config.sceneType.Key == "MainScene")
            {
                Debug.Log("LoadQuestShopper  StartScene()");
                LoadQuestShopper();
                return;
            }

            float internalTime = SceneLoader.IsLevelComplied ? START_INTERNAL_TIME : LEVEL_NOT_COMPLIED_INTERNAL_TIME;
            InitFinishShopper(internalTime);
        }

        public void StartScene(AdvertisementService advertisementService)
        {
            _advertisementService = FindObjectOfType<AdvertisementService>();

            advertisementService.ShowBannerAd();

            string prevScene = PlayerPrefs.GetString(previousScene, "Carrot");
            if (prevScene == "MainScene")
            {
                Debug.Log("LoadQuestShopper  StartScene(AdvertisementService advertisementService)");
                LoadQuestShopper();
                return;
            }

            float internalTime = SceneLoader.IsLevelComplied ? START_INTERNAL_TIME : LEVEL_NOT_COMPLIED_INTERNAL_TIME;
            InitFinishShopper(internalTime);
        }

        // if we got to "QuestScene" from other scenes except for MainScene
        public void InitFinishShopper(float intervalTimeCount)
        {
            if (_finishShopper != null)
                return;

            Debug.Log("InitFinishShopper");
            _finishShopper = CreateFinishShopper(_config);
            DOVirtual.DelayedCall(intervalTimeCount, () =>
            {
                _finishShopper.PlayLeavingSound();
            });
            
            var sequence = DOTween.Sequence();
            sequence.AppendInterval(intervalTimeCount);
            sequence.AppendCallback(() => _finishShopper.StartCarAnimation());
            sequence.Append(_finishShopper.MoveTo(_shoperDestination.position));
            sequence.AppendCallback(LoadQuestShopper);
        }

        // spawn shopper without task
        private Shopper CreateFinishShopper(QuestLevelConfig _config)
        {
            Debug.Log("CreateFinishShopper");
            Shopper shopper = _shopperSpawner.SpawnFinishShopper(_config);
            return shopper;
        }
        
        // if we got to "QuestScene" from "MainScene", spawn shopper with task
        private void LoadQuestShopper()
        {
            _questActor.ChangeShopper();
            Destroy(_finishShopper?.gameObject);
            _finishShopper = null;
        }

    }
}