using ChickenScene.Entities;
using UnityEngine;

namespace ChickenScene
{
    [CreateAssetMenu(fileName = "ChickenLevel", menuName = "Configs/ChickenLevel")]
    public class ChickenLevelConfig : ScriptableObject
    {

        [SerializeField] private WormChicken _worm;
        [SerializeField] private Egg _egg;
        [SerializeField] private ChickenBasket _chickenBasket;
        [SerializeField] private int _wormsToSpawn;
        [SerializeField] private int _eggToWin; // how many eggs need to collect to win
        
       
        public WormChicken Worm => _worm;
        public Egg Egg => _egg;
        public ChickenBasket ChickenBasket => _chickenBasket;
        
        public int WormsToSpawn => _wormsToSpawn;
        public int EggToWin => _eggToWin;

    }
}