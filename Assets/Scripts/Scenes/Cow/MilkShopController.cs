using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;
using UsefulComponents;


namespace CowScene
{
    public class MilkShopController : MonoBehaviour
    {
        private const string MILK_STREAM = "MilkStream";

        public event Action SpawnBottle;
        public event Action AllBottlesFilled;

        [Header("Positions")]
        [SerializeField] private Transform[] _bottlePositions;
        [SerializeField] private Transform _jarSpillingPosition;
        [SerializeField] private Transform _jarBehindShopPosition;
        [SerializeField] private Transform _jarEndPosition;
        [Header("Systems")]
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;
        [Header("Misc")]
        [SerializeField] private CowLevelConfig _levelConfig;

        private CowMilkShop _shop;
        private List<Jar> _jars;
        private List<MilkBottle> _bottles = new List<MilkBottle>();

        private MilkBottle _currentBotle;
        private int _currentBotleCount = 0;

        // Assigns the spawned cow milk shop and subscribes to relevant events
        public void ShopSpawned(CowMilkShop shop)
        {
            _shop = shop;
            _jars = shop.Jars;
            _shop.ShopArrived += SpawnBottleInvoke;
            _shop.BottleMoved += SpawnBottleInvoke;
            _shop.OnJarSpill += SpillJar;
        }

        // Invokes the bottle spawning process
        public void SpawnBottleInvoke()
        {
            if (_currentBotleCount >= _levelConfig.BottleCount)
            {
                _shop.BottleMoved -= SpawnBottleInvoke;
                AllBottlesFilled.Invoke();
                return;
            }
            _currentBotleCount++;
            SpawnBottle?.Invoke();
        }

        // Handles the spawned milk bottle
        public void BottleSpawned(MilkBottle bottle)
        {
            _currentBotle = bottle;
            _bottles.Add(bottle);
            _currentBotle.BottleArrived += OnBottleReady;
        }

        // Called when the milk bottle is ready
        private void OnBottleReady()
        {
            if (_currentBotleCount == 1)
            {
                ShowHint();
            }

            foreach (var jar in _jars)
            {
                jar.MakeInteractable();
            }
        }

        // Spills a jar and fills the bottle with milk
        private void SpillJar(Jar jar)
        {
            _currentBotle.BottleFilled += OnBottleFilled;
            HintSystem.Instance.HidePointerHint();
            jar.EmptyOutJar(_jarBehindShopPosition.position);
            _currentBotle.FillWithMilk();
            _soundSystem.PlaySound(MILK_STREAM);
        }

        // Called when the bottle is filled
        private void OnBottleFilled(MilkBottle bottle)
        {
            _shop.MoveBottleAside(bottle);
            bottle.BottleFilled -= OnBottleFilled;
        }

        // Activates interaction for all bottles
        public void ActivateAllBottles()
        {
            foreach (var bottle in _bottles)
            {
                bottle.MakeInteractable();
            }
        }

        // Shows the hint for the first jar and bottle
        private void ShowHint()
        {
            HintSystem.Instance.ShowPointerHint(_jars[0].transform.position, _bottles[0].transform.position);
        }
    }
}

