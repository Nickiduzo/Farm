using Bee.Spawners;
using AwesomeTools.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UsefulComponents;

namespace Bee
{
    public class HivesController : MonoBehaviour
    {
        private const string BEE_SOUND = "Bee";

        public event Action OnAllHivesOpened;

        [SerializeField] private HiveSpawner _hiveSpawner;
        [SerializeField] private RecyclerSpawner _recyclerSpawner;
        private List<IHive> _hives = new();
        private SoundSystem _soundSystem;
        private Coroutine hintCoroutine;

        // Spawns the hives
        public void SpawnHives(SoundSystem soundSystem, FxSystem fxSystem)
        {
            _soundSystem = soundSystem;
            _hives = _hiveSpawner.SpawnHives(soundSystem, fxSystem);
        }


        // Initialize the hives
        public void InitHives()
        {
            _soundSystem.StopSound(BEE_SOUND);

            foreach (var hive in _hives)
            {
                hive.Init();
                hive.OnOpened += OnOpened;
                hive.OnStored += MakeAllHoneyCombsNonInteractable;
            }

            hintCoroutine = StartCoroutine(ShowHints());
        }

        // Event handler when a hive is opened
        private void OnOpened()
        {
            StopCoroutine(hintCoroutine);
            HintSystem.Instance.HidePointerHint();

            if (!IsAllHivesOpened())
            {
                return;
            }

            OnAllHivesOpened?.Invoke();
            MakeAllHoneyCombsInteractable();
            ActivateHint();
        }

        // Make all honeycombs interactable
        public void MakeAllHoneyCombsInteractable()
        {
            foreach (var hive in _hives)
            {
                hive.HoneyComb.MakeInteractable();
            }
        }

        // Make all honeycombs non-interactable
        public void MakeAllHoneyCombsNonInteractable()
        {
            foreach (var hive in _hives)
            {
                hive.HoneyComb.MakeNonInteractable();
            }
        }

        // Check if all hives are opened
        private bool IsAllHivesOpened()
        {
            return _hives.All(hive => hive.IsOpened);
        }

        // Activate the hint for the next step
        private void ActivateHint()
        {
            var firstHivePosition = _hives[0].StayPosition;
            var recyclerHintPosition = _recyclerSpawner.HintPosition;

            HintSystem.Instance.ShowPointerHint(firstHivePosition + Vector3.up, recyclerHintPosition + Vector3.up);
        }

        // Show hints for each hive in sequence
        private IEnumerator ShowHints()
        {
            while (true)
            {
                for (var i = 0; i < _hives.Count; i++)
                {
                    HintSystem.Instance.ShowPointerHint(_hives[i].StayPosition);
                    yield return new WaitForSeconds(0.5f);
                    HintSystem.Instance.HidePointerHint();
                }
            }
        }


    }
}