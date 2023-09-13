using UnityEngine;


namespace CowScene
{
    [CreateAssetMenu(fileName = "CowLevel", menuName = "Configs/CowLevel")]
    public class CowLevelConfig : ScriptableObject
    {
        [SerializeField] private Cow _cowPrefab;
        [SerializeField] private Hay _hayPrefab;
        [SerializeField] private Jar _jarPrefab;
        [SerializeField] private MilkBottle _bottlePrefab;
        [SerializeField] private CowMilkShop _shopPrafeb;
        [SerializeField] private CollectionArea _bottleBasket;
        [SerializeField] private int _hayCount;
        [SerializeField] private int _jarCount;
        [SerializeField] private int _bottleCount;

        public int HayCount => _hayCount;
        public int JarCount => _jarCount;
        public int BottleCount => _bottleCount;

        public Cow Cow => _cowPrefab;
        public Hay Hay => _hayPrefab;
        public Jar Jar => _jarPrefab;
        public MilkBottle MilkBottle => _bottlePrefab;
        public CowMilkShop CowMilkShop => _shopPrafeb;
        public CollectionArea CollectionArea => _bottleBasket;
    }
}

