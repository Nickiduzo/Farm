using System;
using System.Collections;
using AwesomeTools.Sound;
using UnityEngine;
using UsefulComponents;

namespace Bee
{
    public class GrinderMechanic : MonoBehaviour
    {
        [SerializeField] private float _soundRoutineWaitTime;
        [SerializeField] private float _progressStep;

        private Coroutine _grindingRoutine;
        private Coroutine _soundRoutine;
        private ISoundSystem _soundSystem;
        private string _nameSong;

        [HideInInspector] public AppearAndDisappear _appearAndDisappear;
        public IProgressCalculator progressCalculator;
        public Action GrindingRoutineAction;
        public RotateHint rotateHint;

        private void Awake()
        {
            _appearAndDisappear = rotateHint.GetComponent<AppearAndDisappear>();
            _appearAndDisappear.Disappear();
        }

        public void Init()
        {
            progressCalculator = new FrameProgressCalculator(_progressStep);
        }

        public void Init(ISoundSystem soundSystem, string nameSong)
        {
            _soundSystem = soundSystem;
            _nameSong = nameSong;
        }


        // Start the grinding routine
        public void StartGrinding()
        {
            _grindingRoutine ??= StartCoroutine(GrindingRoutine());
            _soundRoutine ??= StartCoroutine(SoundRoutine());
        }

        // Stop the grinding routine
        public void StopGrinding()
        {
            if (_grindingRoutine != null)
            {
                _soundSystem.StopSound(_nameSong);
                StopCoroutine(_grindingRoutine);
                StopCoroutine(_soundRoutine);                
                _grindingRoutine = null;
                _soundRoutine = null;
            }
        }

        /// <summary>
        /// Routine для звуку переробки
        /// </summary>        
        private IEnumerator SoundRoutine()
        {
            
            while (true)
            {
                _soundSystem.PlaySound(_nameSong);
                yield return new WaitForSeconds(_soundRoutineWaitTime);                
            }
        }

        // Coroutine for the grinding routine
        private IEnumerator GrindingRoutine()
        {
            _soundSystem.PlaySound(_nameSong);
            while (true)
            {
                yield return new WaitForFixedUpdate();
                progressCalculator.AddProgress(Time.fixedDeltaTime);
                GrindingRoutineAction?.Invoke();
                rotateHint.Rotate(Vector3.right);
            }
        }
    }
}