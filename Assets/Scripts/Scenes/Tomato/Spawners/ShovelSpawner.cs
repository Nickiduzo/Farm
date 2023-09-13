using AwesomeTools.Inputs;
using AwesomeTools.Sound;
using UnityEngine;
using Tomato.Spawners.Pools;
namespace Tomato
{
    public class ShovelSpawner : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputSystem _inputSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private TomatoShovelPool _shovelPool;
        [SerializeField] private Transform _spawnPoint;
        [SerializeField] private Transform _destinationPoint;

        private const float X_POS = 0.85f;
        private const float Y_POS = 0.21f;

        /// <summary>
        /// Викликає лопату [shovel]
        /// </summary>
        public TomatoShovel SpawnShovel()
        {
            TomatoShovel shovel = _shovelPool.Pool.GetFreeElement();
            shovel.transform.position = _spawnPoint.position;
            CalculateDestinationPoint();

            shovel.Construct(_destinationPoint.position, _spawnPoint.position, _inputSystem, _soundSystem);
            return shovel;
        }

        /// <summary>
        /// Рахує точку призначення
        /// </summary>
        private void CalculateDestinationPoint()
            => _destinationPoint.position = _camera.ScreenToWorldPoint(new Vector3(X_POS * Screen.width, Y_POS * Screen.height, 1));
    }
}