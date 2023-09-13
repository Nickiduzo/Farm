using AwesomeTools.Sound;
using UnityEngine;

namespace Quest
{
    public class QuestActor : MonoBehaviour
    {
        [SerializeField] private ShopperSpawner _shopperSpawner;
        [SerializeField] private Transform _shopperDestination;
        [SerializeField] private QuestLevelConfig _config;
        [SerializeField] SoundSystem _soundSystem;


        public void StartQuest()
        {
            Shopper shopper = GetShopper();
            _soundSystem.InitLevelMusic();
            MoveToDestinationAndSpawn(shopper);
        }

        public void ChangeShopper()
        {
            Shopper shopper = GetShopper();
            MoveToDestinationAndSpawn(shopper);
        }

        public void StartScene(AdvertisementService advertisementService)
        {
            advertisementService.ShowBannerAd();
            Shopper shopper = GetShopper();
            _soundSystem.InitLevelMusic();
            MoveToDestinationAndSpawn(shopper);
        }

        // Moves shopper to destination and create random task
        private void MoveToDestinationAndSpawn(Shopper shopper)
            => shopper.MoveToAndSpawn(_shopperDestination.position);

        // invoke SpawnQuestShopper in ShopperSpawner
        private Shopper GetShopper()
            => _shopperSpawner.SpawnQuestShopper(_config);

    }
}