using DG.Tweening;
using System;
using UnityEngine;

namespace SunflowerScene
{
    public class SeedPlacer
    {
        private readonly Transform[] _parts;
        private readonly Action AllSeedPlaced;
        private readonly PlacementByOffset _placementByOffset;

        private int _index;

        public SeedPlacer(Transform[] parts, Action allSeedPlaced,PlacementByOffset placementByOffset)
        {
            _parts = parts;
            AllSeedPlaced = allSeedPlaced;
            _placementByOffset = placementByOffset;

        }

        public void Place()
        {
            _parts[_index].DOMove(_placementByOffset.GetPosition(), 0.2f).OnComplete(Next);
            _parts[_index].DOScale(Vector3.one, 0.4f);
        }

        private void Next()
        {
            if (_index + 1 >= _parts.Length)
            {
                AllSeedPlaced?.Invoke();
            }
            else
            {
                _index++;
                Place();
            }
        }
    }
}