using Apple.Spawners;
using AwesomeTools.Sound;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Apple
{
    public class AppleHoleContainer : HolesContainer<AppleHole>
    {
        private const string SUCCESS = "Success";

        [SerializeField] private SoundSystem _soundSystem;

        public event Action OnAllHolesReady;
        public event Action OnAllApplesRipe;
        public event Action OnAllApplesGrew;
        private int _holesReady;

        // It subscribes to events
        private void Awake()
        {
            foreach (var hole in _holesOnScene)
            {
                hole.OnHoleReadyToSeed += CalculateReadyHole;
                hole.OnApplesRipe += CheckAllApplesRipe;
                hole.OnApplesGrew += CheckAllApplesGrew;
            }
        }
        // It unsubscribes from events
        private void OnDestroy()
        {
            foreach (var hole in _holesOnScene)
            {
                hole.OnHoleReadyToSeed -= CalculateReadyHole;
                hole.OnApplesRipe -= CheckAllApplesRipe;
                hole.OnApplesGrew -= CheckAllApplesGrew;
            }
        }

        // Check if all apples in all holes are ripe
        private void CheckAllApplesRipe()
        {
            if (_holesOnScene.All(x => x.IsApplesRipe))
                OnAllApplesRipe?.Invoke();
        }

        // Check if all apples in all holes have grown
        private void CheckAllApplesGrew()
        {
            if (_holesOnScene.All(x => x.IsApplesGrown))
                OnAllApplesGrew?.Invoke();
        }

        // Calculate a ready hole
        private void CalculateReadyHole()
        {
            _holesReady++;
            _soundSystem.PlaySound(SUCCESS);
            if (IsAllHolesReady())
            {
                OnAllHolesReady?.Invoke();
            }
        }

        // Check if all holes are ready
        private bool IsAllHolesReady()
            => _holesReady >= _holesOnScene.Count;

        // Enable all holes
        public void EnableHoles() => _holesOnScene.ForEach(x => x.MakeInteractable());

        // Start the watering hint for all holes
        public void StartWaterHint() => _holesOnScene.ForEach(x => x.StartWateringHint());

        // Get a list of all apples in all holes
        public List<AppleFruit> AllApples()
        {
            List<AppleFruit> apples = new List<AppleFruit>();

            foreach (var hole in _holesOnScene)
            {
                apples.AddRange(hole.Apples());
            }
            return apples;
        }

    }
}