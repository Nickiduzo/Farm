using System;
using UI;
using UnityEngine;
using DG.Tweening;

namespace SunflowerScene
{
    public class Basket : MonoBehaviour, IPointable, IWinInvoker
    {
        [SerializeField] private BasketTriggerObserver _observer;
        [SerializeField] private EntityMover _entityMover;
        [SerializeField] private Transform _startSeedPosition;
        [SerializeField] private PlacementByOffset _endSeedPositions;
        [SerializeField] private Vector3 pointOffset = new Vector3(0, 1.5f);
        private int _needSeeds;
        private int _collectedSeeds;

        public EntityMover Mover => _entityMover;

        public event Action SeedCollected;
        public event Action<Vector3> SeedCollectedIn;
        public event Action AllNeedSeedCollected;
        public event Action OnWin;


        // Initializes the SeedPlanter with the specified number of seeds.
        public void Construct(int seeds)
        {
            _needSeeds = seeds;
            _observer.OnTriggerEnter += HandleSeed;
        }

        // Handles the interaction with a Seed object triggered by collision.
        private void HandleSeed(Seed seed)
        {
            if (seed.Collected == false)
            {
                seed.transform.SetParent(transform);
                seed.transform.DOMove(_startSeedPosition.position, 0.2f).OnComplete(() => PlaceSeed(seed));
            }
        }

        // Places the seed in the designated positions using a SeedPlacer object.
        private void PlaceSeed(Seed seed)
        {
            var seedPlacer = new SeedPlacer(seed.Parts, UpdateSeed, _endSeedPositions);
            seedPlacer.Place();
        }

        // Updates the seed count and invokes the appropriate events.
        private void UpdateSeed()
        {
            _collectedSeeds++;
            OnSeedCollected();
            if (_collectedSeeds == _needSeeds)
            {
                OnAllNeedSeedCollected();
            }
        }

        // Invokes the events when all the needed seeds have been collected.
        protected virtual void OnAllNeedSeedCollected()
        {
            OnWin?.Invoke();
            AllNeedSeedCollected?.Invoke();
        }

        // Invokes the events when a seed is collected.
        private void OnSeedCollected()
        {
            SeedCollected?.Invoke();
            SeedCollectedIn?.Invoke(transform.position);
        }

        // Returns the position of the seed planter point.
        public Vector3 GetPointPosition()
        {
            return transform.position + pointOffset + Vector3.up;
        }
    }
}