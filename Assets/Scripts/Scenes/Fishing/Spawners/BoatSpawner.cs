using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace Fishing.Spawners
{
    public class BoatSpawner : MonoBehaviour
    {
        public Action OnLevelCompleted;
        private const float X_POS = 0.43f;
        private const float Y_POS = 0.6f;
        
        public event Action<Hook, Animator> OnSpawn;

        [SerializeField] private FishLevelConfig _config;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private Camera _camera;

        /// <summary>
        /// Ствроює човен, присвоює дані гачку через метод "Construct" та викликає подію "OnSpawn"
        /// </summary>
        public void SpawnBoat()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));

            GameObject boat = Instantiate(_config.Boat, destination, Quaternion.identity);
            Animator animator = boat.GetComponent<Animator>();
            Hook hook = boat.GetComponentInChildren<Hook>();
            hook.Construct(_inputSystem, _soundSystem, animator);
            OnSpawn?.Invoke(hook, animator);
            OnLevelCompleted += () => StopAnimations(animator);
        }

        /// <summary>
        /// вводимо аніматор [animator]- вимикаємо аніматор [animator]
        /// </summary>        
        private void StopAnimations(Animator animator)
        {
            animator.enabled = false;
        }
    }
}