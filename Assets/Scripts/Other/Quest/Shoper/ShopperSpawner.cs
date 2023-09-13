using UnityEngine;
using AwesomeTools.Sound;

namespace Quest
{
    /// <summary>
    /// Спавнер покупателя
    /// </summary>
    public class ShopperSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _questShopperSpawnPoint;
        [SerializeField] private Transform _finishShopperSpawnPoint;
        [SerializeField] private Transform _basketSpawnPoint;
        [SerializeField] private Shopper _shopper;
        [SerializeField] private TaskSpawner _taskSpawner;
        [SerializeField] private ShopperController _shopperController;
        [SerializeField] private SoundSystem _soundSystem;

        private const string Shopper = "shopper";
        private int shopperID;

        /// <summary>
        /// Вызывает покупателя с заданием
        /// </summary>
        /// <param name="_config">конфиг квеста уровня</param>
        /// <returns></returns>
        public Shopper SpawnQuestShopper(QuestLevelConfig _config)
        {
            shopperID = GetRandomShopper();
            PlayerPrefs.SetInt(Shopper, shopperID);

            Shopper shopper = SpawnShopper(_shopper, _questShopperSpawnPoint.position);
            _shopperController.SetShopper(shopper);
            shopper.Construct(_taskSpawner, shopperID, _soundSystem);
            shopper.SetRandomAnimal(_config);
            return shopper;
        }

        /// <summary>
        /// Вызывает покупателя без задания, и он будет ждать, пока все товары не будут полностью загружены в машину
        /// </summary>
        /// <param name="_config">конфиг квеста уровня</param>
        /// <returns></returns>
        public Shopper SpawnFinishShopper(QuestLevelConfig _config)
        {
            if (_shopperController.IsShopperExist)
                return _shopperController.GetShopper();

            shopperID = PlayerPrefs.GetInt(Shopper, 1);
            Shopper shopper = SpawnShopper(_shopper, _finishShopperSpawnPoint.position);
            _shopperController.SetShopper(shopper);
            shopper.SetSkin(shopperID, _soundSystem);
            shopper.GetAnimal(_config);
            shopper.SetProduct(_config.sceneType, _basketSpawnPoint);
            return shopper;
        }

        /// <summary>
        /// Рандомный костюм для машины
        /// </summary>
        /// <returns>Возвращает рандомный костюм</returns>
        private int GetRandomShopper()
            => Random.Range(0, _shopper.SkinsCount);

        /// <summary>
        /// Создаёт покупателя
        /// </summary>
        /// <param name="toSpawn">покупатель</param>
        /// <param name="position">позиция появления</param>
        /// <returns></returns>
        private Shopper SpawnShopper(Shopper toSpawn, Vector3 position)
            => Instantiate(toSpawn, position, toSpawn.transform.rotation);

    }
}