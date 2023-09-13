using UnityEngine;

namespace Bee.Config
{
    [CreateAssetMenu(fileName = "BeeLevel", menuName = "Configs/BeeLevel")]
    public class BeeLevelConfig : ScriptableObject
    {
        [SerializeField] private CollectionArea _basket;
        [SerializeField] private HoneyGlass _honeyGlass;
        [SerializeField] private HoneyRecycler _honeyRecycler;
        [SerializeField] private Hive[] _hives;
        [SerializeField] private Sprayer _sprayer;
        [SerializeField] private Bee _bee;
        [SerializeField] private int _beeToSpawn;
        [SerializeField] private int _glassToSpawn;

        public CollectionArea Basket => _basket;
        public HoneyGlass HoneyGlass => _honeyGlass;
        public Hive[] Hives => _hives;
        public HoneyRecycler HoneyRecycler => _honeyRecycler;
        public Bee Bee => _bee;
        public Sprayer Sprayer => _sprayer;
        public int BeeToSpawn => _beeToSpawn;
        public int GlassToSpawn => _glassToSpawn;
    }
}