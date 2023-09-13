using UnityEngine;

namespace Tomato
{
    [CreateAssetMenu(fileName = "TomatoLevel", menuName = "Configs/TomatoLevel")]
    public class TomatoLevelConfig : ScriptableObject
    {
        [SerializeField] private CollectionArea tomatoTomatoBasket;
        [SerializeField] private Worm _worm;
        [SerializeField] private WaterPump _waterPump;
        [SerializeField] private TomatoShovel _shovel;
        [SerializeField] private TomatoSeed _tomatoSeed;
        [SerializeField] private WormsBasket wormsBasket;
        [SerializeField] private int _wormsToSpawn;
        [SerializeField] private int _tomatoToSpawn;
        [SerializeField] private int _maxStoredTomato;

        public TomatoShovel Shovel => _shovel;
        public CollectionArea TomatoBasket => tomatoTomatoBasket;
        public WaterPump WateringCan => _waterPump;
        public Worm Worm => _worm;
        public TomatoSeed TomatoSeed => _tomatoSeed;
        public int WormsToSpawn => _wormsToSpawn;
        public WormsBasket WormsBasket => wormsBasket;
        public int MaxStoredTomato => _maxStoredTomato;
    }
}