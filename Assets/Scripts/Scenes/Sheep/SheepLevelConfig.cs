using UnityEngine;

namespace Sheep
{
    [CreateAssetMenu(fileName = "SheepLevel", menuName = "Configs/SheepLevel")]
    public class SheepLevelConfig : ScriptableObject
    {
        [SerializeField] private CollectionArea _testBasket;
        [SerializeField] private ComposedFur _composedFur;
        [SerializeField] private Sheep _sheepPrefab;
        [SerializeField] private Sheep _blackSheepPrefab;
        [SerializeField] private Trimmer _trimmerPrefab;
        [SerializeField] private Shop _shopPrefab;
        [SerializeField] private int _sheepCount;
        [SerializeField] private int _composedFurToSpawn;

        public int SheepCount => _sheepCount;
        public int ComposedFurToSpawn => _composedFurToSpawn;
        public Sheep Sheep => _sheepPrefab;
        public Sheep BlackSheep => _blackSheepPrefab;
        public Trimmer Trimmer => _trimmerPrefab;
        public Shop Shop => _shopPrefab;
        public ComposedFur ComposedFur => _composedFur;
        public CollectionArea CollectionArea => _testBasket;
    }
}