using Carrot.ChainResponsibility;
using DG.Tweening;
using AwesomeTools.Sound;
using System;
using System.Collections.Generic;
using UnityEngine;
using UsefulComponents;
using Random = UnityEngine.Random;

namespace Carrot
{
    public class CarrotHole : BaseHole, IWaterFillable
    {
        private const string Success = "Success";
        public event Action<CarrotHole, Vector3> OnHoleFillWithWater;

        [SerializeField] private FxSystem _fxSystem;
        [SerializeField] private SoundSystem _soundSystem;
        [SerializeField] private SpriteRenderer _fillWaterDirt;
        [SerializeField] private SpriteRenderer _buryDirt;
        [SerializeField] private Transform _moleSpawnPoint;
        [SerializeField] private Transform _seedSpawnPoint;
        [SerializeField] private float _burySize;
        [SerializeField] private float _seedOffSet;
        [SerializeField] private float _framesToFillHoleWithWater;
        [SerializeField] private Transform _carrotAppearPoint;
        [SerializeField] private SpriteMask _holeSpriteMask;
        [SerializeField] private Animation _holeRedAnimation;
        [SerializeField] private Animation _buryDirtRedAnimation;

        public bool IsFull => SeedsCount >= 3;
        public int SeedsCount => _seeds.Count;
        public Vector3 MoleSpawnPosition => _moleSpawnPoint.position;
        public Vector3 SeedDestination => GetSeedDestinationWithOffSet();

        private List<Seed> _seeds = new();

        private bool _isFill;

        private IChainAction _action;


        // Get max progress for hole, initialize Actions, disable sprite mask above hole
        private void Awake()
        {
            _progressCalculator = new FrameProgressCalculator(CalculateMaxProgress());
            InitActions();
            ActivateSpriteMask(false);
            _progressCalculator.OnProgressStep += ExecuteAction;
        }

        // set Actions for [IChainAction]
        private void InitActions()
        {
            _action = new AppearDirt(_fillWaterDirt);
            _action.SetNextAction(new AppearCarrot(this));
        }

        private void OnDestroy()
            => _progressCalculator.OnProgressStep -= ExecuteAction;

        // set max progress
        private float CalculateMaxProgress()
            => _framesToFillHoleWithWater * Time.deltaTime;

        // make seeds part of Carrot hole, enable sprite mask, add each seed in List
        public void AddSeed(Seed seed)
        {
            _seeds.Add(seed);
            ActivateSpriteMask(true);
            PlaySeedSound();
            seed.transform.SetParent(transform);
        }

        // disable sprite mask, appear dirt after putting seeds, make unavaible for tapping, update scale, disable seeds in hole
        public void Bury()
        {
            ActivateSpriteMask(false);
            IChainAction action = new AppearDirt(_buryDirt);
            action.Execute();
            ShowSuccessFx();
            DisposeSeeds();
            MakeNonInteractable();
            transform.DOScale(Vector3.one * _burySize, 0.3f);
        }

        // enable red blinking for hole, 
        public void HoleRedBlink(bool isBlink)
        {
            Animation dirt = new Animation();

            if (_buryDirt.color.a > 0f)
                dirt = _buryDirtRedAnimation;
            else
                dirt = _holeRedAnimation;

            if (isBlink)
            {
                dirt.Play();
            }
            else
            {
                dirt.clip.SampleAnimation(dirt.gameObject, 0f);
                dirt.Stop();
            }
        }

        // enable sprite mask
        public void ActivateSpriteMask(bool isActivate)
            => _holeSpriteMask.enabled = isActivate;

        // set carrot spawn point make it unavailable for interaction
        public void FillWater()
        {
            _isFill = true;
            MakeNonInteractable();
            PlayWaterFillSound();
            ShowSuccessFx();
            OnHoleFillWithWater?.Invoke(this, _carrotAppearPoint.position);
        }

        // invoke [Execete] in [IChainAction]
        private void ExecuteAction()
            => _action.Execute();
        
        // play "Success" sound and FX
        private void ShowSuccessFx()
        {
            _soundSystem.PlaySound("Success");
            _fxSystem.PlayEffect("Success", transform.position);
        }

        // play "Put" sound when seeds is collected in hole 
        private void PlaySeedSound()
            => _soundSystem.PlaySound("Put");

        // play "Success" sound when watering process is complete
        private void PlayWaterFillSound()
            => _soundSystem.PlaySound(Success);

        // set random offSet for destination seeds point inside hole
        private float GetRandomOffSet()
            => Random.Range(-_seedOffSet, _seedOffSet);

        // set Destination point for seeds with offset
        private Vector3 GetSeedDestinationWithOffSet()
            => new(_seedSpawnPoint.position.x + GetRandomOffSet(), _seedSpawnPoint.position.y, 0);

        // disable all seeds in hole
        private void DisposeSeeds()
        {
            foreach (var seed in _seeds)
                seed.gameObject.SetActive(false);
        }

        // disable hole when we take carrot
        public void DestroyHole()
        {
            ShowSuccessFx();
            gameObject.SetActive(false);
        }

        // set private bool [_isFill]
        public bool IsFillWithWater()
            => _isFill;
    }
}