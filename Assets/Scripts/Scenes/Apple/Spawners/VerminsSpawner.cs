using AwesomeTools.Sound;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Apple
{
    public class VerminsSpawner : MonoBehaviour, IProgressWriter
    {
        public event Action OnProgressChanged;

        [SerializeField] private List<Transform> _verminSpawnPositions;
        [SerializeField] private Transform _treePoint;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private VerminPool _verminPool;

        public int KilledVermins { get; private set; }
        public int MaxVermins { get; private set; }

        public int CurrentProgress => KilledVermins;
        public int MaxProgress => MaxVermins;

        private Action OnKilledAllVermins;
        private int _spriteIndex = 0;

        // Initialize and spawn vermins
        public void InitAndSpawn(int maxVermins, Action onKilledAllVermins)
        {
            MaxVermins = maxVermins;
            OnKilledAllVermins = onKilledAllVermins;
            StartCoroutine(SpawnVerminsRoutine());
        }

        // Coroutine for spawning vermins
        private IEnumerator SpawnVerminsRoutine()
        {
            _soundSystem.PlaySound("Vermin");

            for (var i = 0; i < MaxVermins; i++)
            {
                var vermin = SpawnVermin();
                vermin.OnHit += UpdateProgress;
                vermin.SenNewSortingOrder(_spriteIndex);
                _spriteIndex += 6;
                yield return new WaitForSeconds(UnityEngine.Random.Range(1, 2));
            }
        }

        // Update the progress of vermin kills
        private void UpdateProgress()
        {
            ++KilledVermins;
            OnProgressChanged?.Invoke();

            if (KilledVermins >= MaxVermins)
            {
                _soundSystem.StopSound("Vermin");
                OnKilledAllVermins?.Invoke();
            }
        }

        // Spawn a vermin
        public Vermin SpawnVermin()
        {
            var vermin = _verminPool.Pool.GetFreeElement();

            var start = GetRandomSpawnPosition();

            if (start.x > 0)
                vermin.Flip();

            var end = GetRandomEndPosition(start);

            vermin.Construct(start, end, _soundSystem, _treePoint);

            return vermin;
        }

        // Get a random spawn position for vermins
        public Vector3 GetRandomSpawnPosition()
        {
            return _verminSpawnPositions[UnityEngine.Random.Range(0, _verminSpawnPositions.Count)].position;
        }

        // Get a random end position for vermins, excluding the given start position
        public Vector3 GetRandomEndPosition(Vector3 start)
        {
            var pos = _verminSpawnPositions[0].position;

            while (pos.x == start.x)
            {
                pos = GetRandomSpawnPosition();
            }

            return pos;
        }

    }
}