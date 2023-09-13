using UnityEngine;

namespace Apple
{
    [CreateAssetMenu(fileName = "AppleLevel", menuName = "Configs/AppleLevel")]
    public class AppleLevelConfig : ScriptableObject
    {
        [SerializeField] private int _applesToSpawn;
        [SerializeField] private AppleShovel _shovelPrefab;
        [SerializeField] private AppleHole _holeApplePrefab;
        [SerializeField] private SeedlingsApple _seedlingsApplePrefab;
        [SerializeField] private AppleFertilizerPackage _fertilizerPackage;
        [SerializeField] private AppleWaterPump _waterPump;
        [SerializeField] private AppleFruit _apple;
        [SerializeField] private Vermin _vermin;
        [SerializeField] private CollectionArea _appleBasket;
        [SerializeField] private int _maxStoredApples;
        [SerializeField] private int _holesNumber;


        public AppleShovel Shovel => _shovelPrefab;
        public SeedlingsApple SeedlingsApple => _seedlingsApplePrefab;
        public AppleWaterPump WaterPump => _waterPump;
        public AppleFertilizerPackage FertilizerPackage => _fertilizerPackage;
        public AppleFruit Apple => _apple;
        public Vermin Vermin => _vermin;
        public CollectionArea Basket => _appleBasket;
        public int ApplesToSpawn => _applesToSpawn;
        public int MaxStoredApples => _maxStoredApples;

    }
}

