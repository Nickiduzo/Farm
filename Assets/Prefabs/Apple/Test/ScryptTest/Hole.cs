using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Apple
{
    public class Hole : MonoBehaviour
    {
        public event Action<Vector3> OnHoleFillWithWater;

        [SerializeField] private SpriteRenderer _fillWaterDirt;
        [SerializeField] private Collider2D _selfCollider;
        [SerializeField] private Transform _moleSpawnPoint;
        [SerializeField] private Transform _seedSpawnPoint;
        [SerializeField] private float _burySize;
        [SerializeField] private float _seedOffSet;
        [SerializeField] private float _framesToFillHoleWithWater;
        [SerializeField] private Transform _carrotAppearPoint;
        public int SeedsCount => _seeds.Count;
        public bool IsFull => SeedsCount >= 3;
        public Vector3 MoleSpawnPosition => _moleSpawnPoint.position;
        public Vector3 SeedDestination => GetSeedDestinationWithOffSet();

        private float _desiredFillProgress;
        private float _currentFillProgress;

        private List<Seed> _seeds = new();

        private void Awake()
            => _desiredFillProgress = _framesToFillHoleWithWater * Time.deltaTime;

        public void AddSeed(Seed seed)
        {
            _seeds.Add(seed);
            seed.transform.SetParent(transform);
        }

        public void Bury()
        {
            DisposeSeeds();
            _selfCollider.enabled = false;
            transform.DOScale(Vector3.one * _burySize, 0.3f);
        }

        public void AddPourProgress(float progress)
        {
            _currentFillProgress += progress;


            if (IsFillWithWater())
            {
                _selfCollider.enabled = false;
                OnHoleFillWithWater?.Invoke(_carrotAppearPoint.position);
                return;
            }


            if (IsHalfFill())
                _fillWaterDirt.DOFade(1, 0.4f);
        }

        public bool IsFillWithWater()
            => _currentFillProgress >= _desiredFillProgress;

        private float GetRandomOffSet()
            => Random.Range(-_seedOffSet, _seedOffSet);

        private Vector3 GetSeedDestinationWithOffSet()
            => new(_seedSpawnPoint.position.x + GetRandomOffSet(), _seedSpawnPoint.position.y, 0);

        private void DisposeSeeds()
        {
            foreach (var seed in _seeds)
                seed.gameObject.SetActive(false);
        }

        private bool IsHalfFill()
            => _currentFillProgress >= _desiredFillProgress / 2;

        public Tween Disappear()
            => transform.DOScale(Vector3.zero, 0.5f);
    }
}