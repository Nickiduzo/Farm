using DG.Tweening;
using AwesomeTools.Inputs;
using Sheep.Spawners.Pools;
using AwesomeTools.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Sheep.Spawners
{
    public class ComposedFurSpawner : MonoBehaviour
    {
        public event Action OnAllFurComposed;

        private const string SUCCESS_CONST = "Success";
        [SerializeField] private List<float> _offSets;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] FxSystem _fxSystem;
        [SerializeField] SoundSystem _soundSystem;
        [SerializeField] private ComposedFurPool _pool;

        private Vector3 _furStartDestination;
        private Vector3 _furSpawnPoint;
        private List<ComposedFur> _composedFurs = new();
        private int _offsetIndex = 0;
        public void Construct(Vector3 furSpawnPoint, Vector3 furStartDestination)
        {
            _furSpawnPoint = furSpawnPoint;
            _furStartDestination = furStartDestination;
            SetComposedFurList();
        }

        private void SetComposedFurList()
        {
            while (_pool.Pool.HasFreeElement())
            {
                _composedFurs.Add(_pool.Pool.GetFreeElement());
            }
            _composedFurs.Reverse();
        }

        // Spawn a fur
        public void SpawnFur()
        {
            ComposedFur spawnedFur = _composedFurs[_offsetIndex];
            spawnedFur.transform.position = _furSpawnPoint;

            Vector3 furDestination = CalculateFurDestination();

            spawnedFur.Construct(_inputSystem, furDestination, _soundSystem);
            _composedFurs.Add(spawnedFur);

            spawnedFur.MoveTo(furDestination)
                .OnComplete(MakeInteractable);

            NextStep();
        }

        // Perform the next step
        private void NextStep()
            => _offsetIndex++;

        // Calculate the destination for the fur
        private Vector3 CalculateFurDestination()
            => new(_furStartDestination.x + _offSets[_offsetIndex], _furStartDestination.y, _furStartDestination.z);

        // Make the furs interactable
        private void MakeInteractable()
        {
            if (_pool.Pool.HasFreeElement())
            {
                return;
            }

            OnAllFurComposed?.Invoke();

            _fxSystem.PlayEffect(SUCCESS_CONST, _furSpawnPoint);
            _soundSystem.PlaySound(SUCCESS_CONST);

            foreach (var fur in _composedFurs)
            {
                fur.MakeInteractable();
            }
        }

    }
}