using System;
using UnityEngine;
using AwesomeTools;

namespace CowScene
{
    public class Udder : MonoBehaviour
    {
        [SerializeField] private Collider2D _collider;
        [SerializeField] private MouseTrigger _mouseTrigger;
        [SerializeField] private GameObject _jarHintObj;

        public event Action CowMilked;
        public event Action MilkingConcluded;

        // It subscribes from events
        private void Awake()
        {
            _mouseTrigger.OnDown += StartMilking;
        }

        /// <summary>
        /// Видаляє ф-цію "StartMilking" з події "OnDown"
        /// </summary>
        private void OnDestroy()
            => _mouseTrigger.OnDown -= StartMilking;

        /// <summary>
        /// Активує підказку
        /// </summary>
        public void ActivateJarHint()
        {
            _jarHintObj.SetActive(true);
        }
        // Makes the object interactable by enabling its collider and disabling the jar hint
        public void MakeInteractable()
        {
            DisableJarHint();
            _collider.enabled = true;
        }

        // Makes the object non-interactable by disabling its collider
        public void MakeNonInteractable()
        {
            _collider.enabled = false;
        }

        // Disables the jar hint
        public void DisableJarHint()
        {
            _jarHintObj.SetActive(false);
        }

        // Starts the milking process
        private void StartMilking()
        {
            MakeNonInteractable();
            CowMilked?.Invoke();
        }

        // Stops the milking process and enables the jar hint
        public void StopMilking()
        {
            ActivateJarHint();
            MilkingConcluded?.Invoke();
        }
    }

}
