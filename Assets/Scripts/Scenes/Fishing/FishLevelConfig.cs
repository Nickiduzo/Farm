using UnityEngine;

namespace Fishing
{
    [CreateAssetMenu(fileName = "FishLevel", menuName = "Configs/FishLevel")]
    public class FishLevelConfig : ScriptableObject
    {
        [SerializeField] private int _fishSpawnCount;
        [SerializeField] private Fish _fish;
        [SerializeField] private FishingNet _fishingNet;
        [SerializeField] private CollectionArea _basket;
        [SerializeField] private GameObject _boat;

        public int FishSpawnCount => _fishSpawnCount;
        public Fish Fish => _fish;
        public FishingNet Net => _fishingNet;
        public GameObject Boat => _boat;
        public CollectionArea Basket => _basket;
    }
}