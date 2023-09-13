using System.Collections;
using DG.Tweening;
using AwesomeTools.Sound;
using UnityEngine;

namespace Sheep
{
    // Trimmer blade that interact with fur scratches
    public class TrimmerBlade : MonoBehaviour
    {
        private const int TRIMMER_BTN_CHANGE_POS = 180;
        public bool IsTrimmerHasAnimator = false;
        private const string Trimmer = "Trimmer";
        private const string FUR_FX_CONST = "FurFx";
        [SerializeField] private Transform _trimmerBtnTrans;
        [SerializeField] private Trimmer _trimmer;
        [SerializeField] private float _shakeDuration;
        [SerializeField] private int _randomness;
        [SerializeField] private float _shakingStrenght;
        [SerializeField] private int _shakingVibrato;
        [SerializeField] private float _showingFxCoolDown;
        [SerializeField] private FurTriggerObserver _furTriggerObserver;
        [SerializeField] private FurTriggerObserver _furTriggerObserverEnabler;

        private bool _isProcessingFur;
        private Coroutine _trimFurCoroutine;
        private Tween _shakingTween;
        private Quaternion _cachedRotation;
        private ISoundSystem _soundSystem;
        private FxSystem _fxSystem;

        // It subscribes to events
        private void Awake()
        {
            _furTriggerObserver.OnTriggerEnter += ProcessFur;
            _furTriggerObserverEnabler.OnTriggerEnter += AddFurTrimmedCount;
            _furTriggerObserverEnabler.OnTriggerExit += InterruptFurProcessing;
            _cachedRotation = transform.parent.rotation;
        }

        // It unsubscribes from events
        private void OnDestroy()
        {
            _furTriggerObserver.OnTriggerEnter -= ProcessFur;
            _furTriggerObserverEnabler.OnTriggerEnter -= AddFurTrimmedCount;
            _furTriggerObserverEnabler.OnTriggerExit -= InterruptFurProcessing;
        }

        // get systems
        public void Construct(ISoundSystem soundSystem, FxSystem fxSystem)
        {
            _soundSystem = soundSystem;
            _fxSystem = fxSystem;
        }


        // Process the fur for trimming
        private void ProcessFur(FurScratch fur)
        {
            if (fur.IsTrimmed)
            {
                return;
            }
            SheepAnimator.IsFurTrimming = true;
            _trimmer.GainAccessToShowTongue();
            fur.Shake();
            if (_isProcessingFur) return;

            _trimmerBtnTrans.eulerAngles += new Vector3(0,0,TRIMMER_BTN_CHANGE_POS);
            _soundSystem.PlaySound(Trimmer);
            _isProcessingFur = true;
            _trimFurCoroutine = StartCoroutine(TrimFurRoutine(fur));

            if (!IsTrimmerHasAnimator)
            {
                _trimmer.SetAnimator(fur.GetSheepAnimator());
                IsTrimmerHasAnimator = true;
            }
        }
        /// <summary>
        /// Збільшує значення "FurTriggerObserver.FurAmountTrimmed"
        /// </summary>        
        private void AddFurTrimmedCount(FurScratch fur)
        {
            FurTriggerObserver.FurAmountTrimmed++;
        }

        // Interrupt the fur processing
        private void InterruptFurProcessing(FurScratch fur)
        {
            FurTriggerObserver.FurAmountTrimmed--;

            fur.ShakingTween.Kill();
            _shakingTween.Kill();
            if (FurTriggerObserver.FurAmountTrimmed == 0)
            {                
                _trimmerBtnTrans.eulerAngles -= new Vector3(0,0,TRIMMER_BTN_CHANGE_POS);
                SheepAnimator.IsFurTrimming = false;
                _isProcessingFur = false;
                _soundSystem.StopSound(Trimmer);

                if (_trimFurCoroutine != null)
                {
                    StopCoroutine(_trimFurCoroutine);
                }
            }

            RotateToCached();
        }

        // Shake the object
        private Tween Shake()
            => transform.parent.DOShakeRotation(_shakeDuration, new Vector3(_shakingStrenght, 0, 0), _shakingVibrato, _randomness, true, ShakeRandomnessMode.Harmonic);

        // Rotate the object to the cached rotation
        private void RotateToCached()
            => transform.parent.DORotateQuaternion(_cachedRotation, _shakeDuration);

        // Trim the fur routine
        private IEnumerator TrimFurRoutine(FurScratch fur)
        {
            while (_isProcessingFur)
            {
                _fxSystem.PlayEffect(FUR_FX_CONST, transform.position);
                _shakingTween = Shake();
                yield return new WaitForSeconds(_showingFxCoolDown);
            }
        }

    }
}