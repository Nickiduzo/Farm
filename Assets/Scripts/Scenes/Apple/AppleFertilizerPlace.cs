using System;
using DG.Tweening;
using UnityEngine;
using UsefulComponents;

namespace Apple
{
    [RequireComponent(typeof(Animator))]
    public class AppleFertilizerPlace : BaseHole, IDiggable
    {
        private const string BLICK_KEY = "Blink";

        [SerializeField] private SpriteRenderer _dirt;
        [SerializeField] private float _framesToFill;

        private Animator Animator { get; set; }

        private bool _fertilized;
        private IChainAction _action;

        public event Action OnFertilized;

        public bool IsFertilized
        {
            get => _fertilized;
            set
            {
                if (!_fertilized)
                {
                    SetDirtBlink();
                }
                _fertilized = value;
            }
        }

        // subscribe to the event and init actions
        private void Awake()
        {
            Animator = GetComponent<Animator>();

            InitActions();
            _progressCalculator = new FrameProgressCalculator(CalculateMaxProgress());
            _progressCalculator.OnProgressStep += ExecuteAction;
        }

        // Initialize DirtHalfScale and set new actions 
        private void InitActions()
        {
            _action = new DirtHalfScale(_dirt);
            _action.SetNextAction(new DirtDisappear(_dirt, this));
        }

        // Execute the current action
        private void ExecuteAction()
            => _action.Execute();

        // Store the seed in the dirt
        public void StoreSeed()
        {
            MakeNonInteractable();
            _fertilized = true;
            SetDirtVisible();
            OnFertilized?.Invoke();
        }

        // Set up the dirt to be ready for fertilization
        public void SetupToFertilize()
        {
            _dirt.gameObject.SetActive(true);
            SetDirtBlink();
        }

        // Dig the dirt
        public void Dig()
        {
            Animator.SetBool(BLICK_KEY, true);
        }

        // Calculate the maximum progress
        private float CalculateMaxProgress()
            => Time.deltaTime * _framesToFill;

        // Set the dirt to blink
        private void SetDirtBlink()
        {
            Animator.SetBool(BLICK_KEY, true);
        }

        // Set the dirt to be visible
        private void SetDirtVisible()
        {
            Animator.SetBool(BLICK_KEY, false);
        }

        // Make the dirt partially visible
        public void HalfVisibility()
        {
            _dirt.material.DOFade(_dirt.material.color.a - 0.33f, 0.5f);
        }


    }
}