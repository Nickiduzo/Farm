using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace Apple.Spawners
{
    public class ShovelSpawner : MonoBehaviour
    {
        public event Action<AppleShovel> OnShovelSpawned;

        [SerializeField] private Camera _camera;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private FxSystem _fxSystem;
        [SerializeField] private AppleShovelPool _shovelPool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;
        [SerializeField] private Transform _holePos;
        
        private const float X_POS = 0.8f;
        private const float Y_POS = 0.33f;

        // Spawn a shovel
        public AppleShovel SpawnShovel()
        {
            AppleShovel shovel = _shovelPool.Pool.GetFreeElement();
            shovel.transform.position = _spawnPoint.position;
            CalculateDestinationPoint();

            OnShovelSpawned?.Invoke(shovel);

            shovel.Construct(_destinationPoint.position, _spawnPoint.position, _inputSystem, _soundSystem, _fxSystem, _holePos.position);
            return shovel;
        }

        // Calculate the destination point for the shovel
        private void CalculateDestinationPoint()
            => _destinationPoint.position = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
    }
}

