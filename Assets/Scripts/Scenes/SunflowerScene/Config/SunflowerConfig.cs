using UnityEngine;

namespace SunflowerScene
{
    [CreateAssetMenu(fileName = "SunflowerLevel", menuName = "Configs/SunflowerLevel")]
    public class SunflowerConfig : ScriptableObject
    {
        [SerializeField] private Car _car;
        [SerializeField] private Sunflower _sunflower;
        [SerializeField] private WaterPump _waterPump;
        [SerializeField] private CollectionArea _basket;
        [SerializeField] private Pest _pest;
        [SerializeField] private Seed _seed;
        [SerializeField] private int _maxStoredSeeds;
        public Car Car => _car;
        public WaterPump WaterPump => _waterPump;
        public CollectionArea Basket => _basket;
        public Pest Pest => _pest;
        public Seed Seed => _seed;
        public Sunflower Sunflower => _sunflower;
        public int MaxStoredSeeds => _maxStoredSeeds;
    }
}