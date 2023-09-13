using AwesomeTools.Sound;
using System;
using UnityEngine;

namespace Fishing.Spawners
{
    public class FishingNetSpawner : MonoBehaviour
    {
        private const float X_POS = 4.7f;
        private const float Y_POS = 1;

        public event Action<FishingNet> OnSpawn;

        [SerializeField] private FishingNetPool _pool;
        [SerializeField] private Transform _netSpawnPosition;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private Camera _camera;
        
        /// <summary>
        /// Присвоюємо сітці необхідні дані в ф-ції "Construct", позицію появи та викликаю подію "OnSpawn"
        /// </summary>
        public void SpawnNet()
        {
            var destination = _camera.ScreenToWorldPoint(new Vector3((X_POS * Screen.width) / 5, (Y_POS * Screen.height) / 2, 1));

            FishingNet net = _pool.Pool.GetFreeElement();
            net.transform.position = _netSpawnPosition.position;
            net.Construct(destination, _soundSystem);
            OnSpawn?.Invoke(net);
        }
    }
}