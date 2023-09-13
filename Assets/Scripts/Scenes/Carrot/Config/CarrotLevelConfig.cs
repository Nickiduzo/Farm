using UnityEngine;

namespace Carrot.Config
{
    [CreateAssetMenu(fileName = "CarrotLevel", menuName = "Configs/CarrotLevel")]
    public class CarrotLevelConfig : ScriptableObject
    {
        [SerializeField] private CollectionArea _basket;
        [SerializeField] private Carrot _carrot;
        [SerializeField] private Mole _mole;
        [SerializeField] private WaterPump _waterPump;
        [SerializeField] private SeedPackage _seedPackage;
        [SerializeField] private Seed _seed;
        [SerializeField] private int _maxMoleToSpawn;
        [SerializeField] private int _catchMoleToWin;
        
        public Mole Mole => _mole;
        public Carrot Carrot => _carrot;
        public WaterPump WaterPump => _waterPump;
        public SeedPackage SeedPackage => _seedPackage;
        public int MaxMoleToSpawn => _maxMoleToSpawn;
        public Seed Seed => _seed;
        public CollectionArea CollectionArea => _basket;

        public int CatchMoleToWin => _catchMoleToWin;
    }
}
