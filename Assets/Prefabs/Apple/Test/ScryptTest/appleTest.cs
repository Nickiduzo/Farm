using UnityEngine;

namespace Apple
{
    [CreateAssetMenu(fileName = "TestLevel", menuName = "Configs/TestLevel")]
    public class appleTest : ScriptableObject
    {
        [SerializeField] private CarrotBasket _basket;
        [SerializeField] private Carrot _carrot;
        [SerializeField] private Mole _mole;
        [SerializeField] private SeedPackage _seedPackage;
        [SerializeField] private Seed _seed;
        [SerializeField] private int _maxMoleToSpawn;
        public Mole Mole => _mole;
        public CarrotBasket Basket => _basket;
        public Carrot Carrot => _carrot;
        public SeedPackage SeedPackage => _seedPackage;
        public int MaxMoleToSpawn => _maxMoleToSpawn;
        public Seed Seed => _seed;
    }
}

